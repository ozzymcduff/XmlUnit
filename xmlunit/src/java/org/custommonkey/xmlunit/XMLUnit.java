package org.custommonkey.xmlunit;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.FactoryConfigurationError;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.parsers.SAXParserFactory;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.TransformerConfigurationException;
import javax.xml.transform.TransformerFactoryConfigurationError;
import java.io.IOException;
import java.io.Reader;
import java.io.StringReader;
import org.w3c.dom.Document;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;

/**
 * Allows access to project control parameters such as which Parser to use and
 * provides some convenience methods for building Documents from Strings etc.
 * <br />Examples and more at <a href="http://xmlunit.sourceforge.net"/>xmlunit.sourceforge.net</a>
 */
public final class XMLUnit {
    private static DocumentBuilderFactory controlBuilderFactory;
    private static DocumentBuilderFactory testBuilderFactory;
    private static TransformerFactory transformerFactory;
    private static boolean ignoreWhitespace = false;

    private static final String STRIP_WHITESPACE_STYLESHEET
        = new StringBuffer(XMLConstants.XML_DECLARATION)
        .append(XSLTConstants.XSLT_START)
        .append(XSLTConstants.XSLT_XML_OUTPUT_NOINDENT)
        .append(XSLTConstants.XSLT_STRIP_WHITESPACE)
        .append(XSLTConstants.XSLT_IDENTITY_TEMPLATE)
        .append(XSLTConstants.XSLT_END)
        .toString();

    /**
     * Private constructor.
     * Makes class non-instantiable
     */
    private XMLUnit() {
        // access via static methods please
    }

    /**
     * Overide the DocumentBuilder to use to parse control documents.
     * This is useful when comparing the output of two different
     * parsers. Note: setting the control parser before any test cases
     * are run will affect the test parser as well.
     */
    public static void setControlParser(String className)
    throws FactoryConfigurationError {
        System.setProperty("javax.xml.parsers.DocumentBuilderFactory", className);
        controlBuilderFactory = getControlDocumentBuilderFactory();
    }
    /**
     * Get the <code>DocumentBuilder</code> instance used to parse the control
     * XML in an XMLTestCase.
     * @return parser for control values
     * @throws ParserConfigurationException
     */
    public static DocumentBuilder getControlParser()
    throws ParserConfigurationException {
        if (controlBuilderFactory == null) {
            controlBuilderFactory = getControlDocumentBuilderFactory();
        }
        return controlBuilderFactory.newDocumentBuilder() ;
    }

    /**
     * Get the <code>DocumentBuilderFactory</code> instance used to instantiate
     * parsers for the control XML in an XMLTestCase.
     * @return factory for control parsers
     */
    public static DocumentBuilderFactory getControlDocumentBuilderFactory() {
        if (controlBuilderFactory == null) {
            controlBuilderFactory = DocumentBuilderFactory.newInstance();
            controlBuilderFactory.setNamespaceAware(true);
        }
        return controlBuilderFactory;
    }
    /**
     * Override the <code>DocumentBuilderFactory</code> used to instantiate
     * parsers for the control XML in an XMLTestCase.
     */
    public static void setControlDocumentBuilderFactory(DocumentBuilderFactory factory) {
        if (factory == null) {
            throw new IllegalArgumentException("Cannot set control DocumentBuilderFactory to null!");
        }
        controlBuilderFactory = factory;
    }

    /**
     * Overide the DocumentBuilder to use to parser test documents.
     * This is useful when comparing the output of two different
     * parsers. Note: setting the test parser before any test cases
     * are run will affect the control parser as well.
     */
    public static void setTestParser(String className)
    throws FactoryConfigurationError {
        System.setProperty("javax.xml.parsers.DocumentBuilderFactory", className);
        testBuilderFactory = getTestDocumentBuilderFactory();
    }
    /**
     * Get the <code>DocumentBuilder</code> instance used to parse the test XML
     * in an XMLTestCase.
     * @return parser for test values
     * @throws ParserConfigurationException
     */
    public static DocumentBuilder getTestParser()
    throws ParserConfigurationException {
        if (testBuilderFactory == null) {
            testBuilderFactory = getTestDocumentBuilderFactory();
        }
        return testBuilderFactory.newDocumentBuilder();
    }

    /**
     * Get the <code>DocumentBuilderFactory</code> instance used to instantiate
     * parsers for the test XML in an XMLTestCase.
     * @return factory for test parsers
     */
    public static DocumentBuilderFactory getTestDocumentBuilderFactory() {
        if (testBuilderFactory == null) {
            testBuilderFactory = DocumentBuilderFactory.newInstance();
            testBuilderFactory.setNamespaceAware(true);
        }
        return testBuilderFactory;
    }
    /**
     * Override the <code>DocumentBuilderFactory</code> used to instantiate
     * parsers for the test XML in an XMLTestCase.
     */
    public static void setTestDocumentBuilderFactory(DocumentBuilderFactory factory) {
        if (factory == null) {
            throw new IllegalArgumentException("Cannot set test DocumentBuilderFactory to null!");
        }
        testBuilderFactory = factory;
    }

    /**
     * Whether to ignore whitespace when comparing node values.
     * This method also invokes <code>setIgnoringElementContentWhitespace()</code>
     *  on the underlying control AND test document builder factories.
     */
    public static void setIgnoreWhitespace(boolean ignore){
        ignoreWhitespace = ignore;
        getControlDocumentBuilderFactory().setIgnoringElementContentWhitespace(ignore);
        getTestDocumentBuilderFactory().setIgnoringElementContentWhitespace(ignore);
    }

    /**
     * Whether to ignore whitespace when comparing node values.
     * @return true if whitespace should be ignored when comparing nodes, false
     * otherwise
     */
    public static boolean getIgnoreWhitespace(){
        return ignoreWhitespace;
    }

    /**
     * Compare XML documents provided by two Reader classes.
     * @param control Control document
     * @param test Document to test
     * @return Diff object describing differences in documents
     * @throws SAXException
     * @throws IOException
     * @throws ParserConfigurationException
     * @decremented use Diff constructor directly
     */
    public static Diff compare(Reader control, Reader test) throws SAXException,
    IOException, ParserConfigurationException {
        return new Diff(control, test);
    }

    /**
     * Compare two XML documents provided by a string and a Reader.
     * @param control Control document
     * @param test Document to test
     * @return Diff object describing differences in documents
     * @throws SAXException
     * @throws IOException
     * @throws ParserConfigurationException
     * @decremented use Diff constructor directly
     */
    public static Diff compare(String control, Reader test) throws SAXException,
    IOException, ParserConfigurationException {
        return new Diff(new StringReader(control), test);
    }

    /**
     * Compare two XML documents provided by a Reader and a string.
     * @param control Control document
     * @param test Document to test
     * @return Diff object describing differences in documents
     * @throws SAXException
     * @throws IOException
     * @throws ParserConfigurationException
     * @decremented use Diff constructor directly
     */
    public static Diff compare(Reader control, String test) throws SAXException,
    IOException, ParserConfigurationException {
        return new Diff(control, new StringReader(test));
    }

    /**
     * Compare two XML documents provided as strings.
     * @param control Control document
     * @param test Document to test
     * @return Diff object describing differences in documents
     * @throws SAXException
     * @throws IOException
     * @throws ParserConfigurationException
     * @decremented use Diff constructor directly
     */
    public static Diff compare(String control, String test) throws SAXException,
    IOException, ParserConfigurationException {
        return new Diff(control, test);
    }

    /**
     * Utility method to build a Document using the control DocumentBuilder
     * to parse the specified String.
     * @param fromXML
     * @return Document representation of the String content
     * @throws SAXException
     * @throws IOException
     * @throws ParserConfigurationException
     */
    public static Document buildControlDocument(String fromXML)
    throws SAXException, IOException, ParserConfigurationException {
        return buildDocument(getControlParser(), new StringReader(fromXML));
    }

    /**
     * Utility method to build a Document using the test DocumentBuilder
     * to parse the specified String.
     * @param fromXML
     * @return Document representation of the String content
     * @throws SAXException
     * @throws IOException
     * @throws ParserConfigurationException
     */
    public static Document buildTestDocument(String fromXML)
    throws SAXException, IOException, ParserConfigurationException {
        return buildDocument(getTestParser(), new StringReader(fromXML));
    }

    /**
     * Utility method to build a Document using a specific DocumentBuilder
     * and reading characters from a specific Reader.
     * @param withBuilder
     * @param fromReader
     * @return Document built
     * @throws SAXException
     * @throws IOException
     */
    protected static Document buildDocument(DocumentBuilder withBuilder,
    Reader fromReader) throws SAXException, IOException {
        return withBuilder.parse(new InputSource(fromReader));
    }

    /**
     * Overide the transformer to use for XSLT transformations (and by
     * implication serialization and XPaths).
     * This is useful when comparing transformer implementations.
     * @throws TransformerFactoryConfigurationError
     */
    public static void setTransformerFactory(String className)
    throws TransformerFactoryConfigurationError {
        System.setProperty("javax.xml.transform.TransformerFactory",
            className);
        transformerFactory = null;
        getTransformerFactory();
    }

    /**
     * Get the transformer to use for XSLT transformations (and by
     * implication serialization and XPaths).
     * @return the current transformer factory in use
     * @throws TransformerFactoryConfigurationError if unable to construct
     * a new instance of the default transformer factory
     * @throws TransformerFactoryConfigurationError
     */
    public static TransformerFactory getTransformerFactory()
    throws TransformerFactoryConfigurationError {
        if (transformerFactory == null) {
            transformerFactory = TransformerFactory.newInstance();
        }
        return transformerFactory;
    }


    /**
     * Override the SAX parser to use in tests.
     * Currently only used by {@link Validator Validator class}
     * @param className
     */
    public static void setSAXParserFactory(String className) {
        System.setProperty("javax.xml.parsers.SAXParserFactory", className);
        getSAXParserFactory();
    }

    /**
     * Get the SAX parser to use in tests.
     * @return the SAXParserFactory instance used by the {@link Validator Validator}
     * to perform DTD validation
     */
    public static SAXParserFactory getSAXParserFactory() {
        SAXParserFactory newFactory = SAXParserFactory.newInstance();
        newFactory.setNamespaceAware(true);
        return newFactory;
    }

    /**
     * Obtain the transformation that will strip whitespace from a DOM containing
     *  empty Text nodes
     * @param forDocument
     * @return a <code>Transform</code> to do the whitespace stripping
     * @throws TransformerConfigurationException
     */
    public static Transform getStripWhitespaceTransform(Document forDocument)
    throws TransformerConfigurationException {
        return new Transform(forDocument, STRIP_WHITESPACE_STYLESHEET);
    }

    /**
     * Place holder for current version info.
     * @return current version
     */
    public static String getVersion() {
        return "0.6";
    }

}