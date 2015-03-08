namespace XmlUnit {
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    
    public class XmlOutput {

        private readonly XslCompiledTransform _transform;
    	private readonly XsltArgumentList _xsltArgs;
        private readonly XmlReader _navigator;
    	private readonly XmlResolver _resolverForXmlTransformed;
    	private readonly XmlReader[] _readersToClose;
    	
        internal XmlOutput(XslCompiledTransform transform, XsltArgumentList xsltArgs, 
            XmlReader navigator, XmlResolver resolverForXmlTransformed,
	        XmlReader[] readersToClose) {
			_transform = transform;
			_xsltArgs = xsltArgs;
			_navigator = navigator;
			_resolverForXmlTransformed = resolverForXmlTransformed;
			_readersToClose = readersToClose;
		}
		
		private void CleanUp() {
			for (int i = 0; i < _readersToClose.Length; ++i) {
	            _readersToClose[i].Close();
			}
		}
    	                   	 	                   
		public string AsString() {
            var stringWriter = new StringWriter(); 
            var writer = XmlWriter.Create(stringWriter);
            Write(writer);
			return stringWriter.ToString();
		}
		
		public XmlInput AsXml() {
	        return new XmlInput(AsString());
		}
		
		public void Write(XmlWriter viaXmlWriter) {			
	        _transform.Transform(_navigator, _xsltArgs, viaXmlWriter, _resolverForXmlTransformed);
			CleanUp();
		}
    }
}
