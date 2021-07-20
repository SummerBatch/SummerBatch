# Summer Batch

Summer Batch is a lightweight, reliable, efficient, open-source (Summer Batch is distributed under the [Apache 2.0 license](http://www.apache.org/licenses/LICENSE-2.0 "Apache 2 license") ) batch framework for the C# community.

Its design has been driven by the concepts exposed in the [JSR-352](https://www.jcp.org/en/jsr/detail?id=352 "jsr-352 spec") specification (which is a java colored but exposes universal batch concepts).

Purpose of Summer Batch:

* Build new batch solutions to be run on Microsoft®-based environments
* Migrate smoothly their batch legacy from mainframe to modern Microsoft®-based environments

Some of its key features are:

*	Repeatable and customizable batch jobs
*	Multi step jobs, with simple step sequences or conditional logic between them
*	In-memory or persisted job repository
*	Support for a Read-Process-Write logic, as well as arbitrary batchlet for a more complete control on the behavior
*	Chunk processing with checkpoint management and restartability
*	Step partitioning used for parallel processing
*	Database readers and writers, with support for Microsoft® SQL Server, IMB® DB2 and Oracle® databases
*	Flat file readers and writers
*	Easy mapping between the readers and writers and your domain classes
*	Batch contexts at step level and job level
*	XML design for the main batch architecture, C# design for the properties


# Get the source and build Summer Batch

You need to use Visual Studio, version >= 2013 to build Summer Batch from the sources.

Clone the git repository (using whatever protocol you prefer).

Simply build the downloaded solution using VS. 

# Get the binaries

The simplest way to get your hands over Summer Batch binaries is to use the [NuGet package](https://www.nuget.org/packages/SummerBatch/ "nuget package") repository.

# Getting started

To get started with your first Summer Batch project, please follow the 
[getting started guide](https://www.bluage.com/site_docs/summerbatch/docs/v_1-26/site/getting_started_summerbatch/getting_started.html "Getting Started").
  
This will lead you, in a few minutes, to get your first Summer Batch project up and working.

You can browse the [samples repository](https://github.com/SummerBatch/samples "samples repository") to get your hands over
sample Summer Batch projects.

# Going further
To dig deeper into Summer Batch, we recommend to browse the following sites :

*	[Summer Batch main site](https://www.bluage.com/summerbatch "Summer Batch main site")
*	[Reference Guide](https://www.bluage.com/site_docs/summerbatch/docs/v_1-26/site/index.html "Reference Guide")
*	[Summer Batch API Doc](https://www.bluage.com/site_docs/summerbatch/docs/api/index.html "API Doc")

# Want to Contribute ?
You can use the github Issues appliance to ask for improvements or to file bugs you might encounter.
To take any other contact (pull requests, questions, etc), please write to the provided email address. 

Or

1. Fork it
2. Create your feature branch: git checkout -b my-new-feature
3. Commit your changes: git commit -am 'Add some feature'
4. Push to the branch: git push origin my-new-feature
5. Submit a pull request
