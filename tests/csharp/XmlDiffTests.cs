namespace XmlUnit.Tests {
    using XmlUnit;
    using NUnit.Framework;
    using System.IO;
    
    [TestFixture]
    public class XmlDiffTests {
        private XmlDiff _xmlDiff;
          
        [Test] public void EqualResultForSameReader() { 
            TextReader reader = new StringReader("<empty/>");
            DiffResult result = PerformDiff(reader, reader);
            Assert.AreEqual(true, result.Equal);
        }
        
        [Test] public void SameResultForTwoInvocations() {
            TextReader reader = new StringReader("<empty/>");
            DiffResult result1 = PerformDiff(reader, reader);
            DiffResult result2 = _xmlDiff.Compare();
            Assert.AreSame(result1, result2);
            
        }
        
        private void AssertExpectedResult(string input1, string input2, bool expected) {
            TextReader reader1 = new StringReader(input1);
            TextReader reader2 = new StringReader(input2);
            DiffResult result = PerformDiff(reader1, reader2);
            string msg = string.Format("comparing {0} to {1}: {2}", input1,
                                       input2, result.Difference);
            Assert.AreEqual(expected, result.Equal, msg);
        }
        
        private void AssertExpectedResult(string[] inputs1, string[] inputs2, bool expected) {
            for (int i=0; i < inputs1.Length; ++i) {
                AssertExpectedResult(inputs1[i], inputs2[i], expected);
                AssertExpectedResult(inputs2[i], inputs1[i], expected);
                
                AssertExpectedResult(inputs1[i], inputs1[i], true);
                AssertExpectedResult(inputs2[i], inputs2[i], true);
            }
        }
        
        private DiffResult PerformDiff(TextReader reader1, TextReader reader2) {            
            _xmlDiff = new XmlDiff(reader1, reader2);
            DiffResult result = _xmlDiff.Compare();
            return result;
        }
        
        [Test] public void EqualResultForSameEmptyElements() { 
            string[] input1 = {"<empty/>", "<elem><empty/></elem>"};
            string[] input2 = {"<empty></empty>", "<elem><empty></empty></elem>"};
            AssertExpectedResult(input1, input2, true);
        }

        [Test] public void EqualResultForEmptyElementsWithAttributes() { 
            string[] input1 = {"<empty x='1'/>", "<elem><empty x='1'/></elem>"};
            string[] input2 = {"<empty x='1'></empty>", "<elem><empty x='1'></empty></elem>"};
            AssertExpectedResult(input1, input2, true);
        }

        [Test] public void NotEqualResultForEmptyVsNotEmptyElements() { 
            string[] input1 = {"<empty/>" , "<empty></empty>", "<empty><empty/></empty>"};
            string[] input2 = {"<empty>text</empty>", "<empty>text</empty>", "<empty>text</empty>"};
            AssertExpectedResult(input1, input2, false);
        }
        
        [Test] public void NotEqualResultForDifferentElements() { 
            string[] input1 = {"<a><b/></a>" , "<a><b/></a>", "<a><b/></a>"};
            string[] input2 = {"<b><a/></b>", "<a><c/></a>", "<a><b><c/></b></a>"};
            AssertExpectedResult(input1, input2, false);
        }
        
        [Test] public void NotEqualResultForDifferentNumberOfAttributes() { 
            string[] input1 = {"<a><b x=\"1\"/></a>", "<a><b x=\"1\"/></a>"};
            string[] input2 = {"<a><b/></a>", "<a><b x=\"1\" y=\"2\"/></a>"};
            AssertExpectedResult(input1, input2, false);
        }
        
        [Test] public void NotEqualResultForDifferentAttributeValues() { 
            string[] input1 = {"<a><b x=\"1\"/></a>", "<a><b x=\"1\" y=\"2\"/></a>"};
            string[] input2 = {"<a><b x=\"2\"/></a>", "<a><b x=\"1\" y=\"3\"/></a>"};
            AssertExpectedResult(input1, input2, false);
        }
        
        [Test] public void NotEqualResultForDifferentAttributeNames() { 
            string[] input1 = {"<a><b x=\"1\"/></a>", "<a><b x=\"1\" y=\"2\"/></a>"};
            string[] input2 = {"<a><b y=\"2\"/></a>", "<a><b x=\"1\" z=\"3\"/></a>"};
            AssertExpectedResult(input1, input2, false);
        }
        
        [Test] public void EqualResultForDifferentAttributeSequences() { 
            string[] input1 = {"<a x=\"1\" y=\"2\" z=\"3\"/>", 
                                "<a><b x=\"1\" y=\"2\"/></a>"};
            string[] input2 = {"<a y=\"2\" z=\"3\" x=\"1\"/>", 
                                "<a><b y=\"2\" x=\"1\"/></a>"};
            AssertExpectedResult(input1, input2, true);
        }
        
        [Test] public void NotEqualResultForDifferentAttributeValuesAndSequences() { 
            string[] input1 = {"<a x=\"1\" y=\"2\" z=\"3\"/>", 
                                "<a><b x=\"1\" y=\"2\"/></a>"};
            string[] input2 = {"<a y=\"2\" z=\"3\" x=\"2\"/>", 
                                "<a><b y=\"1\" x=\"1\"/></a>"};
            AssertExpectedResult(input1, input2, false);
        }
        
        [Test] public void NotEqualResultForDifferentTextElements() {
            string[] input1 = {"<a>text</a>", "<a>text<b>more text</b></a>", 
                                "<a><b>text</b>more text</a>"};
            string[] input2 = {"<a>some text</a>", "<a>text<b>text</b></a>", 
                                "<a>more text<b>text</b></a>"};
            AssertExpectedResult(input1, input2, false);
        }
        
        [Test] public void CanDistinguishElementClosureAndEmptyElement() {
            string[] input1 = {"<a><b>text</b></a>", "<a>text<b>more text</b></a>"};
            string[] input2 = {"<a><b/>text</a>", "<a>text<b/>more text</a>"};
            AssertExpectedResult(input1, input2, false);
            
        }
        
        [Test] public void NotEqualResultForDifferentLengthElements() {
            string[] input1 = {"<a>text</a>", "<a><b>text</b><c>more text</c></a>"};
            string[] input2 = {"<a>text<b/></a>", "<a><b>text</b>more text<c/></a>"};
            AssertExpectedResult(input1, input2, false);
        }
        
        [Test] public void NamespaceAttributeDifferences() {
            string control = "<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                + "<ns0:Message xmlns:ns0 = \"http://mynamespace\">"
                + "<ns0:EventHeader>"
                + "<ns0:EventID>9999</ns0:EventID>"
                + "<ns0:MessageID>1243409665297</ns0:MessageID>"
                + "<ns0:MessageVersionID>1.0</ns0:MessageVersionID>"
            + "<ns0:EventName>TEST-EVENT</ns0:EventName>"
                + "<ns0:BWDomain>TEST</ns0:BWDomain>"
                + "<ns0:DateTimeStamp>2009-01-01T12:00:00</ns0:DateTimeStamp>"
                + "<ns0:SchemaPayloadRef>anything</ns0:SchemaPayloadRef>"
                + "<ns0:MessageURI>anything</ns0:MessageURI>"
                + "<ns0:ResendFlag>F</ns0:ResendFlag>"
                + "</ns0:EventHeader>"
                + "<ns0:EventBody>"
                + "<ns0:XMLContent>"
                + "<xyz:root xmlns:xyz=\"http://test.com/xyz\">"
                + "<xyz:test1>A</xyz:test1>"
                + "<xyz:test2>B</xyz:test2>"
                + "</xyz:root>"
                + "</ns0:XMLContent>"
                + "</ns0:EventBody>"
                + "</ns0:Message>";
            string test =
                "<abc:Message xmlns:abc=\"http://mynamespace\" xmlns:xyz=\"http://test.com/xyz\">"
                + "<abc:EventHeader>"
                + "<abc:EventID>9999</abc:EventID>"
                + "<abc:MessageID>1243409665297</abc:MessageID>"
                + "<abc:MessageVersionID>1.0</abc:MessageVersionID>"
                + "<abc:EventName>TEST-EVENT</abc:EventName>"
                + "<abc:BWDomain>TEST</abc:BWDomain>"
                + "<abc:DateTimeStamp>2009-01-01T12:00:00</abc:DateTimeStamp>"
                + "<abc:SchemaPayloadRef>anything</abc:SchemaPayloadRef>"
                + "<abc:MessageURI>anything</abc:MessageURI>"
                + "<abc:ResendFlag>F</abc:ResendFlag>"
                + "</abc:EventHeader>"
                + "<abc:EventBody>"
                + "<abc:XMLContent>"
                + "<xyz:root>"
                + "<xyz:test1>A</xyz:test1>"
                + "<xyz:test2>B</xyz:test2>"
                + "</xyz:root>"
                + "</abc:XMLContent>"
                + "</abc:EventBody>"
                + "</abc:Message>";
            AssertExpectedResult(control, test, true);
        }

    }
}
