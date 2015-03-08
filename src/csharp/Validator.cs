namespace XmlUnit {
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    
    public class Validator {
        private bool hasValidated = false;
        private bool isValid = true;
        private string validationMessage;
        private readonly XmlReader validatingReader;
    	
    	private Validator(XmlReader xmlInputReader) {
            var settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema | ValidationType.DTD;
            settings.ValidationEventHandler += ValidationFailed;
            validatingReader = XmlReader.Create(xmlInputReader,settings);          
    	}

        public Validator(XmlInput input) :
        	this(input.CreateXmlReader()) {
        }
                                
        public void ValidationFailed(object sender, ValidationEventArgs e) {
            isValid = false;
            validationMessage = e.Message;
        }
        
        private void Validate() {
            if (!hasValidated) {
                hasValidated = true;
                while (validatingReader.Read()) {
                    // only interested in ValidationFailed callbacks
                }
                validatingReader.Close();
            }
        }
        
        public bool IsValid {
            get {
                Validate();
                return isValid;
            }
        }
        
        public string ValidationMessage {
            get {
                Validate();
                return validationMessage;
            }
            
        }
    }
}
