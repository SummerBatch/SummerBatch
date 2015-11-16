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
//  distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.CoreTests.Util.Test;
using Summer.Batch.Extra.Utils;
using System.Collections.Generic;

namespace Summer.Batch.CoreTests.Util
{
    [TestClass]
    public sealed class ReverseUtilsTests
    {

        #region Test ConvertBooleanToStringON, ConvertBooleanToStringYN.

        ///<summary>
	    /// ReverseUtils.convertBooleanToStringON(Boolean) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_ConvertBooleanToStringON1() 
        {
		    bool? boolean = true;
		    string result = ReverseUtils.ConvertBooleanToStringON(boolean);
		    Assert.AreEqual("O", result);
	    }

	    ///<summary>
	    /// ReverseUtils.ConvertBooleanToStringON(bool?) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_ConvertBooleanToStringON2() 
        {
		    bool? boolean = false;
		    string result = ReverseUtils.ConvertBooleanToStringON(boolean);
		    Assert.AreEqual("N", result);
	    }

	    ///<summary>
	    /// ReverseUtils.ConvertBooleanToStringON(bool?) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_ConvertBooleanToStringON3() 
        {
		    bool? boolean = null;
		    string result = ReverseUtils.ConvertBooleanToStringON(boolean);
		    Assert.AreEqual(null, result);
	    }

	    ///<summary>
	    /// ReverseUtils.ConvertBooleanToStringYN(bool?) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_ConvertBooleanToStringYN1() 
        {
		    bool? boolean = true;
		    string result = ReverseUtils.ConvertBooleanToStringYN(boolean);
		    Assert.AreEqual("Y", result);
	    }

	    ///<summary>
	    /// ReverseUtils.ConvertBooleanToStringYN(bool?) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_ConvertBooleanToStringYN2() 
        {
		    bool? boolean = false;
		    string result = ReverseUtils.ConvertBooleanToStringYN(boolean);
		    Assert.AreEqual("N", result);
	    }

	    ///<summary>
	    /// ReverseUtils.ConvertBooleanToStringYN(bool?) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_ConvertBooleanToStringYN3() 
        {
		    bool? boolean = null;
		    string result = ReverseUtils.ConvertBooleanToStringYN(boolean);
		    Assert.AreEqual(null, result);
	    }

        #endregion

        #region Test ConvertStringToBooleanON, ConvertStringToBooleanYN.

        ///<summary>
	    /// ReverseUtils.ConvertStringToBooleanON(String) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_ConvertStringToBooleanON1() 
        {
		    string stringValue = "N";
		    bool? result = ReverseUtils.ConvertStringToBooleanON(stringValue);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(false, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.ConvertStringToBooleanON(String) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_ConvertStringToBooleanON2() 
        {
		    string stringValue = "";
		    bool? result = ReverseUtils.ConvertStringToBooleanON(stringValue);
		    Assert.AreEqual(null, result);
	    }

	    ///<summary>
	    /// ReverseUtils.ConvertStringToBooleanON(String) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_ConvertStringToBooleanON3() 
        {
		    string stringValue = "O";
		    bool? result = ReverseUtils.ConvertStringToBooleanON(stringValue);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(true, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.ConvertStringToBooleanON(String) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_ConvertStringToBooleanON4() 
        {
		    string stringValue = null;
		    bool? result = ReverseUtils.ConvertStringToBooleanON(stringValue);
		    Assert.AreEqual(null, result);
	    }

	    ///<summary>
	    /// ReverseUtils.ConvertStringToBooleanYN(String) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_ConvertStringToBooleanYN1() 
        {
		    string stringValue = "N";
		    bool? result = ReverseUtils.ConvertStringToBooleanYN(stringValue);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(false, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.ConvertStringToBooleanYN(String) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_ConvertStringToBooleanYN2() 
        {
		    string stringValue = "";
		    bool? result = ReverseUtils.ConvertStringToBooleanYN(stringValue);
		    Assert.AreEqual(null, result);
	    }

	    ///<summary>
	    /// ReverseUtils.ConvertStringToBooleanYN(String) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_ConvertStringToBooleanYN3() 
        {
		    string stringValue = "Y";
		    bool? result = ReverseUtils.ConvertStringToBooleanYN(stringValue);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(true, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.ConvertStringToBooleanYN(String) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_ConvertStringToBooleanYN4() 
        {
		    string stringValue = null;
		    bool? result = ReverseUtils.ConvertStringToBooleanYN(stringValue);
		    Assert.AreEqual(null, result);
	    }

        #endregion

        #region Test IsSpaces.

        ///<summary>
	    /// ReverseUtils.IsSpaces(object) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsSpaces1() 
        {
		    object objet = new object();
		    bool? result = ReverseUtils.IsSpaces(objet);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(true, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.IsSpaces(object) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsSpaces2() 
        {
		    object objet = new object();
		    bool? result = ReverseUtils.IsSpaces(objet);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(true, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.IsSpaces(object) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsSpaces3() 
        {
		    object objet = new object();
		    bool? result = ReverseUtils.IsSpaces(objet);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(true, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.IsSpaces(object) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsSpaces4() 
        {
		    object objet = new object();
		    bool? result = ReverseUtils.IsSpaces(objet);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(true, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.IsSpaces(object) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsSpaces5() 
        {
		    object objet = new object();
		    bool? result = ReverseUtils.IsSpaces(objet);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(true, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.IsSpaces(object) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsSpaces6() 
        {
		    object objet = new object();
		    bool? result = ReverseUtils.IsSpaces(objet);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(true, result.Value);
	    }

        #endregion

        #region Test IsCharLowValue, IsCharHighValue, IsStringLowValue, IsStringHighValue.

    	    ///<summary>
	    /// ReverseUtils.IsCharHighValue(char) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsCharHighValue1() 
        {
		    char character = '\u009F';
		    bool? result = ReverseUtils.IsCharHighValue(character);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(true, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.IsCharHighValue(char) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsCharHighValue2() 
        {
		    char character = '!';
		    bool? result = ReverseUtils.IsCharHighValue(character);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(false, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.IsCharLowValue(char) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsCharLowValue1() 
        {
		    char character = '\u0000';
		    bool? result = ReverseUtils.IsCharLowValue(character);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(true, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.IsCharLowValue(char) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsCharLowValue2() 
        {
		    char character = '!';
		    bool? result = ReverseUtils.IsCharLowValue(character);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(false, result.Value);
	    }

        ///<summary>
	    /// ReverseUtils.IsStringHighValue(String) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsStringHighValue1() 
        {
		    string s = "";
		    bool? result = ReverseUtils.IsStringHighValue(s);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(false, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.IsStringHighValue(String) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsStringHighValue2() 
        {
		    string s = "" + '\u009F' + '\u009F';
		    bool? result = ReverseUtils.IsStringHighValue(s);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(true, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.IsStringHighValue(String) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsStringHighValue3() 
        {
		    string s = "a";
		    bool? result = ReverseUtils.IsStringHighValue(s);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(false, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.IsStringHighValue(String) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsStringHighValue4() 
        {
		    string s = "a" + '\u009F';
		    bool? result = ReverseUtils.IsStringHighValue(s);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(false, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.IsStringLowValue(String) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsStringLowValue1() 
        {
		    string s = "";
		    bool? result = ReverseUtils.IsStringLowValue(s);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(false, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.IsStringLowValue(String) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsStringLowValue2() 
        {
		    string s = "" + '\u0000';
		    bool? result = ReverseUtils.IsStringLowValue(s);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(true, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.IsStringLowValue(String) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsStringLowValue3() 
        {
		    string s = "" + '\u0000' + '\u0000';
		    bool? result = ReverseUtils.IsStringLowValue(s);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(true, result.Value);
	    }

	    ///<summary>
	    /// ReverseUtils.IsStringLowValue(String) method test.
	    ///</summary>
	    [TestMethod]
	    public void ReverseUtils_IsStringLowValue4() 
        {
		    string s = "a" + '\u0000';
		    bool? result = ReverseUtils.IsStringLowValue(s);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(false, result.Value);
	    }

        #endregion
        
    }
}
