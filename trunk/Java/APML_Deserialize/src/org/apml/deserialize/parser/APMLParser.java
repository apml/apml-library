package org.apml.deserialize.parser;

import java.io.File;
import java.net.MalformedURLException;
import java.net.URL;

import org.w3c.dom.NamedNodeMap;
import org.w3c.dom.Node;
import org.w3c.dom.traversal.NodeFilter;
import org.xml.sax.SAXException;
import org.apache.xerces.parsers.DOMParser;
import org.apache.xerces.dom.NodeIteratorImpl;
import org.apache.xerces.dom.DocumentImpl;
import java.io.IOException;
import org.apml.deserialize.model.APML;
import org.apml.deserialize.model.Body;
import org.apml.deserialize.model.Concept;
import org.apml.deserialize.model.DateCreated;
import org.apml.deserialize.model.ExplicitData;
import org.apml.deserialize.model.Generator;
import org.apml.deserialize.model.Head;
import org.apml.deserialize.model.ImplicitData;
import org.apml.deserialize.model.Profile;
import org.apml.deserialize.model.Source;
import org.apml.deserialize.model.Title;
import org.apml.deserialize.model.UserEmail;

public class APMLParser
{
	public static final String APML_SPEC_VERSION = "0.6";
	private DOMParser parser = null;
	private String fileLocation = null;
	
	public APMLParser(File file) throws MalformedURLException
	{
		this.parser = new DOMParser();
		this.fileLocation = file.getAbsolutePath();
	}
	
	public APMLParser(String fileLocation)
	{
		this.parser = new DOMParser();
		this.fileLocation = fileLocation;
	}
	
	public APMLParser(URL url)
	{
		this.parser = new DOMParser();
		this.fileLocation = url.toString();
	}
	
	public APML deserialize()
	{		
		// TODO Finish reading all nodes
		// TODO Check for nulls
		
		DocumentImpl document = null;
		Node root = null;
		NodeIteratorImpl nIterator = null;
		Node n = null;
		APML apmlDoc = null;
		
		try
		{
			this.parser = new DOMParser();
			parser.parse(this.fileLocation);
			document = (DocumentImpl) this.parser.getDocument();
			root = document.getLastChild();
			nIterator = (NodeIteratorImpl)document.createNodeIterator(root, NodeFilter.SHOW_ELEMENT, null, true);
			n = null;

			// Iterate through the nodes
			while ((n = nIterator.nextNode()) != null)
			{
				String nodeName = n.getNodeName();

				if(nodeName.equals("APML"))
					apmlDoc = new APML(n.getAttributes().getNamedItem("version").getNodeValue());
				else if(nodeName.equals("Head"))
					apmlDoc.setHead(new Head());
				else if(nodeName.equals("Title"))
					apmlDoc.getHead().setTitle(new Title(n.getTextContent()));
				else if(nodeName.equals("Generator"))
					apmlDoc.getHead().setGenerator(new Generator(n.getTextContent()));
				else if(nodeName.equals("UserEmail"))
					apmlDoc.getHead().setUserEmail(new UserEmail(n.getTextContent()));
				else if(nodeName.equals("DateCreated"))
					apmlDoc.getHead().setDateCreated(new DateCreated(n.getTextContent()));
				else if(nodeName.equals("UserEmail"))
					apmlDoc.getHead().setUserEmail(new UserEmail(n.getTextContent()));
				else if(nodeName.equals("Body"))
					apmlDoc.setBody(new Body(n.getAttributes().getNamedItem("defaultprofile").getNodeValue()));
				else if(nodeName.equals("Profile"))
					apmlDoc.getBody().getProfiles().put(n.getAttributes().getNamedItem("name").getNodeValue(), new Profile());
				else if(nodeName.equals("ImplicitData"))
				{
					Profile p = (Profile) apmlDoc.getBody().getProfiles().get(n.getParentNode().getAttributes().getNamedItem("name").getNodeValue());
					p.setImplicitData(new ImplicitData());
				}
				else if(nodeName.equals("ExplicitData"))
				{
					Profile p = (Profile) apmlDoc.getBody().getProfiles().get(n.getParentNode().getAttributes().getNamedItem("name").getNodeValue());
					p.setExplicitData(new ExplicitData());

				}
				else if(nodeName.equals("Concept"))
				{
					NamedNodeMap nMap = n.getAttributes();
					String key = nMap.getNamedItem("key").getNodeValue();
					String value = nMap.getNamedItem("value").getNodeValue();
					String uri = "";  // Placeholder for future spec
					String from = nMap.getNamedItem("from").getNodeValue();
					String updated = nMap.getNamedItem("updated").getNodeValue();
					
					Profile p = (Profile) apmlDoc.getBody().getProfiles().get(n.getParentNode().getParentNode().getParentNode().getAttributes().getNamedItem("name").getNodeValue());
					if(n.getParentNode().getParentNode().getNodeName().equals("ImplicitData"))
						p.getImplicitData().getConcepts().add(new Concept(key, value, uri, from, updated));
					else if(n.getParentNode().getParentNode().getNodeName().equals("ExplicitData"))
						p.getExplicitData().getConcepts().add(new Concept(key, value, uri, from, updated));
				}
				else if(nodeName.equals("Source"))
				{
					NamedNodeMap nMap = n.getAttributes();
					String key = nMap.getNamedItem("key").getNodeValue();
					String name = nMap.getNamedItem("name").getNodeValue();
					String value = nMap.getNamedItem("value").getNodeValue();
					String type = nMap.getNamedItem("type").getNodeValue();
					String from = nMap.getNamedItem("from").getNodeValue();
					String updated = nMap.getNamedItem("updated").getNodeValue();
					
					Profile p = (Profile) apmlDoc.getBody().getProfiles().get(n.getParentNode().getParentNode().getParentNode().getAttributes().getNamedItem("name").getNodeValue());
					if(n.getParentNode().getParentNode().getNodeName().equals("ImplicitData"))
						p.getImplicitData().getSources().add(new Source(key, name, value, type, from, updated));
					else if(n.getParentNode().getParentNode().getNodeName().equals("ExplicitData"))
						p.getExplicitData().getSources().add(new Source(key, name, value, type, from, updated));
				}
				else if(nodeName.equals("Application"))
				{
					
				}
			}
		}	
	
		catch(IOException ioEx)
		{
			ioEx.printStackTrace();
		}	
	
		catch(SAXException saxEx)
		{
			saxEx.printStackTrace();
		}
		
		finally
		{
			document = null;
			root = null;
			nIterator = null;
			n = null;
		}
		return apmlDoc;
	}
}
