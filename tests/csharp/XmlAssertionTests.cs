namespace XmlUnit.Tests {
    using XmlUnit;
    using NUnit.Framework;
    using System.IO;
    
    [TestFixture]
    public class XmlAssertionTests {        
        public static readonly string VALID_FILE = "BookXsdGenerated.xml";
        public static readonly string INVALID_FILE = "invalidBook.xml";
        [Test] public void AssertStringEqualAndIdenticalToSelf() {
            string control = "<assert>true</assert>";
            string test = "<assert>true</assert>";
            XmlAssertion.AssertXmlIdentical(control, test);
            XmlAssertion.AssertXmlEquals(control, test);
        }        
        
        [Test] public void AssertDifferentStringsNotEqualNorIdentical() {
            string control = "<assert>true</assert>";
            string test = "<assert>false</assert>";
            XmlDiff xmlDiff = new XmlDiff(control, test);
            XmlAssertion.AssertXmlNotIdentical(xmlDiff);
            XmlAssertion.AssertXmlNotEquals(xmlDiff);
        }        
        
        [Test] public void AssertXmlIdenticalUsesOptionalDescription() {
            string description = "An Optional Description";
            bool caughtException = true;
            try {
                XmlDiff diff = new XmlDiff(new XmlInput("<a/>"), new XmlInput("<b/>"), 
                                           new DiffConfiguration(description));
                XmlAssertion.AssertXmlIdentical(diff);
                caughtException = false;
            } catch (NUnit.Framework.AssertionException e) {
              Assert.IsTrue(e.Message.IndexOf(description) > -1);
            }
            Assert.IsTrue(caughtException);
        }
        
        [Test] public void AssertXmlEqualsUsesOptionalDescription() {
            string description = "Another Optional Description";
            bool caughtException = true;
            try {
                XmlDiff diff = new XmlDiff(new XmlInput("<a/>"), new XmlInput("<b/>"), 
                                           new DiffConfiguration(description));
                XmlAssertion.AssertXmlEquals(diff);
                caughtException = false;
            } catch (NUnit.Framework.AssertionException e) {
                Assert.IsTrue(e.Message.IndexOf(description) > -1);
            }
            Assert.IsTrue(caughtException);
        }
                    
        private StreamReader GetStreamReader(string file) {
            FileStream input = File.Open(file, FileMode.Open, FileAccess.Read);
            return new StreamReader(input);
        }
        
        private static readonly string MY_SOLAR_SYSTEM = "<solar-system><planet name='Earth' position='3' supportsLife='yes'/><planet name='Venus' position='4'/></solar-system>";
        
        [Test] public void AssertXPathExistsWorksForExistentXPath() {
            XmlAssertion.AssertXPathExists("//planet[@name='Earth']", 
                                           MY_SOLAR_SYSTEM);
        }
        
        [Test] public void AssertXPathExistsFailsForNonExistentXPath() {
            bool caughtException = true;
            try {
                XmlAssertion.AssertXPathExists("//star[@name='alpha centauri']", 
                                               MY_SOLAR_SYSTEM);
                caughtException = false;
            } catch (AssertionException e) {
                AvoidUnusedVariableCompilerWarning(e);
            }
            Assert.IsTrue(caughtException);
        }
        
        [Test] public void AssertXPathEvaluatesToWorksForMatchingExpression() {
            XmlAssertion.AssertXPathEvaluatesTo("//planet[@position='3']/@supportsLife", 
                                                MY_SOLAR_SYSTEM,
                                                "yes");
        }
        
        [Test] public void AssertXPathEvaluatesToWorksForNonMatchingExpression() {
            XmlAssertion.AssertXPathEvaluatesTo("//planet[@position='4']/@supportsLife", 
                                                MY_SOLAR_SYSTEM,
                                                "");
        }
        
        [Test] public void AssertXPathEvaluatesToWorksConstantExpression() {
            XmlAssertion.AssertXPathEvaluatesTo("true()", 
                                                MY_SOLAR_SYSTEM,
                                                "True");
            XmlAssertion.AssertXPathEvaluatesTo("false()", 
                                                MY_SOLAR_SYSTEM,
                                                "False");
        }

        private void AvoidUnusedVariableCompilerWarning(AssertionException e) {
            string msg = e.Message;
        }
    }
}
