//
//   Copyright 2015 Blu Age Corporation - Plano, Texas
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using Summer.Batch.Common.TaskExecution;
using Summer.Batch.Core.Job.Builder;
using Summer.Batch.Core.Job.Flow;
using Summer.Batch.Core.Repository;
using Summer.Batch.Core.Unity.Xml;

namespace Summer.Batch.Core.Unity
{
    /// <summary>
    /// Class to handle the registration of jobs.
    /// Xml parsing of the configuration must be have been done prior to
    /// invoking methods from this class.
    /// </summary>
    public class JobRegistrationHandler
    {
        private readonly IDictionary<string, XmlFlow> _xmlFlowMap = new Dictionary<string, XmlFlow>();
        private readonly IDictionary<string, XmlSplit> _xmlSplitMap = new Dictionary<string, XmlSplit>();
        private readonly IDictionary<string, IStep> _stepsMap = new Dictionary<string, IStep>();
        private readonly IDictionary<string, IFlow> _flowMap = new Dictionary<string, IFlow>();
        private readonly IDictionary<string, IFlow> _splitMap = new Dictionary<string, IFlow>();

        private ITaskExecutor _executor;

        /// <summary>
        /// Main job Loader method, it loads the job corresponding to the xml file and 
        /// stores it in the unity container.
        /// </summary>
        /// <param name="unityContainer"></param>
        /// <param name="xmlJob"></param>
        public void LoadJob(IUnityContainer unityContainer, XmlJob xmlJob)
        {
            JobBuilder jobBuilder = new JobBuilder(xmlJob.Id);
            jobBuilder.Repository(unityContainer.Resolve<IJobRepository>());
            jobBuilder.Incrementer(unityContainer.Resolve<IJobParametersIncrementer>());
            if ("false".Equals(xmlJob.Restartable))
            {
                jobBuilder.PreventRestart();
            }
            if (xmlJob.Listeners != null)
            {
                foreach (var listener in xmlJob.Listeners.Listeners)
                {
                    jobBuilder.Listener(unityContainer.Resolve<IJobExecutionListener>(listener.Ref));
                }
            }
            MapXmlElements(unityContainer, xmlJob);

            IJob job = LoadJob(xmlJob, jobBuilder);
            unityContainer.RegisterInstance(xmlJob.Id, job);
        }

        private IJob LoadJob(XmlJob xmlJob, JobBuilder jobBuilder)
        {
            FlowBuilder<FlowJobBuilder> jobFlowBuilder;
            XmlJobElement xmlElement = xmlJob.JobElements.First();
            if (xmlElement is XmlStep)
            {
                jobFlowBuilder = jobBuilder.Flow(_stepsMap[xmlElement.Id]);
            }
            else {
                XmlFlow xmlFlow = xmlElement as XmlFlow;
                XmlSplit xmlSplit = xmlElement as XmlSplit;

                jobFlowBuilder = jobBuilder.Start(xmlFlow != null ? LoadFlow(xmlFlow) : LoadSplit(xmlSplit));
            }

            HandleSubElements(jobFlowBuilder, xmlJob);
            return jobFlowBuilder.End().Build();
        }

        private IFlow LoadSplit(XmlSplit xmlSplit)
        {
            string splitId = xmlSplit.Id;
            if (_splitMap.ContainsKey(splitId))
            {
                return _splitMap[splitId];
            }

            // Using the first element as argument in Start method
            // And all the others (using Skip(1)) in .Add() method
            FlowBuilder<IFlow> flowBuilder = new FlowBuilder<IFlow>(splitId)
                .Start(LoadFlow(xmlSplit.Flows.First()))
                .Split(_executor)
                .Add(xmlSplit.Flows.Skip(1).Select(LoadFlow).ToArray());
            
            IFlow flow = flowBuilder.End();
            _splitMap[xmlSplit.Id] = flow;
            return flow;
        }

        private IFlow LoadFlow(XmlFlow xmlFlow)
        {
            string flowId = xmlFlow.Id;
            if (_flowMap.ContainsKey(flowId))
            {
                return _flowMap[flowId];
            }

            FlowBuilder<IFlow> flowBuilder = new FlowBuilder<IFlow>(flowId);
            XmlJobElement firstElement = xmlFlow.JobElements.First();
            if (firstElement is XmlStep)
            {
                flowBuilder.Start(_stepsMap[firstElement.Id]);
            }
            else {
                XmlFlow xmlSubFlow = firstElement as XmlFlow;
                XmlSplit xmlSplit = firstElement as XmlSplit;

                flowBuilder.Start(xmlSubFlow != null ? LoadFlow((XmlFlow) firstElement) : LoadSplit(xmlSplit));
            }

            HandleSubElements(flowBuilder, xmlFlow);

            IFlow flow = flowBuilder.End();
            _flowMap[xmlFlow.Id] = flow;
            return flow;
        }

        private FlowBuilder<T> HandleSubElements<T>(FlowBuilder<T> builder, IXmlStepContainer container)
        {
            foreach (var xmlElement in container.JobElements)
            {
                XmlFlow xmlSubFlow = xmlElement as XmlFlow;
                XmlStep xmlStep = xmlElement as XmlStep;
                XmlSplit xmlSplit = xmlElement as XmlSplit;

                if (xmlElement.Next != null)
                {
                    Next(From(builder, xmlStep, xmlSubFlow, xmlSplit), xmlElement.Next);
                }
                else if (xmlElement.Transitions.Any())
                {
                    foreach (var transition in xmlElement.Transitions)
                    {
                        var transitionBuilder = From(builder, xmlStep, xmlSubFlow, xmlSplit).On(transition.On);
                        var xmlNext = transition as XmlNext;
                        if (xmlNext != null)
                        {
                            To(transitionBuilder, xmlNext.To);
                        }
                        else if (transition is XmlEnd)
                        {
                            transitionBuilder.End();
                        }
                        else if (transition is XmlFail)
                        {
                            transitionBuilder.Fail();
                        }
                    }
                }
            }
            return builder;
        }

        private FlowBuilder<T> From<T>(FlowBuilder<T> builder, XmlStep xmlStep, XmlFlow xmlFlow, XmlSplit xmlSplit)
        {
            if (xmlStep != null)
            {
                return builder.From(_stepsMap[xmlStep.Id]);
            }
            
            if (xmlFlow != null)
            {
                return builder.From(LoadFlow(xmlFlow));
            }
            
            if (xmlSplit != null)
            {
                return builder.From(LoadSplit(xmlSplit));
            }

            // Won't happen
            return null;
        }

        private FlowBuilder<T> Next<T>(FlowBuilder<T> builder, string next)
        {
            if (_stepsMap.ContainsKey(next))
            {
                return builder.Next(_stepsMap[next]);
            }
            
            if (_xmlFlowMap.ContainsKey(next))
            {
                return builder.Next(LoadFlow(_xmlFlowMap[next]));
            }
            
            if (_xmlSplitMap.ContainsKey(next))
            {
                return builder.Next(LoadSplit(_xmlSplitMap[next]));
            }

            // Won't happen
            return null;
        }

        private FlowBuilder<T> To<T>(FlowBuilder<T>.TransitionBuilder builder, string next)
        {
            if (_stepsMap.ContainsKey(next))
            {
                return builder.To(_stepsMap[next]);
            }
            
            if (_xmlFlowMap.ContainsKey(next))
            {
                return builder.To(LoadFlow(_xmlFlowMap[next]));
            }
            
            if (_xmlSplitMap.ContainsKey(next))
            {
                return builder.To(LoadSplit(_xmlSplitMap[next]));
            }

            // Won't happen
            return null;
        }

        private void MapXmlElements(IUnityContainer unityContainer, IXmlStepContainer xmlStepContainer)
        {
            foreach (var xmlJobElement in xmlStepContainer.JobElements)
            {
                if (xmlJobElement is XmlStep)
                {
                    var xmlStep = (XmlStep) xmlJobElement;
                    var stepId = xmlStep.Id;
                    var step = new StepLoader(xmlStep, unityContainer).LoadStep();
                    _stepsMap[stepId] = step;
                }
                else if (xmlJobElement is XmlFlow)
                {
                    var xmlFlow = (XmlFlow) xmlJobElement;
                    _xmlFlowMap[xmlFlow.Id] = xmlFlow;
                    MapXmlElements(unityContainer, xmlFlow);
                }
                else if (xmlJobElement is XmlSplit)
                {
                    var xmlSplit = (XmlSplit) xmlJobElement;
                    _xmlSplitMap[xmlSplit.Id] = xmlSplit;
                    _executor = unityContainer.Resolve<ITaskExecutor>();
                    foreach (var xmlFlow in xmlSplit.Flows)
                    {
                        MapXmlElements(unityContainer, xmlFlow);
                    }
                }
            }
        }
    }
}