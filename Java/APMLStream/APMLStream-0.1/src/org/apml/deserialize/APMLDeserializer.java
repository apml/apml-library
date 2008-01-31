/*	
 *  APMLStream: Java library for APML serialization/deserialization
 *	By:  Tim Schultz
 *
 *	Licensed under the Apache License, Version 2.0 (the "License"); 
 *	you may not use this file except in compliance with the License. 
 *	You may obtain a copy of the License at 
 *
 *	http://www.apache.org/licenses/LICENSE-2.0 
 *
 *	Unless required by applicable law or agreed to in writing, software 
 *	distributed under the License is distributed on an "AS IS" BASIS, 
 *	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 *	See the License for the specific language governing permissions and 
 *	limitations under the License.
 *
 */

package org.apml.deserialize;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.net.URL;
import java.net.URLConnection;
import java.util.Scanner;
import org.apml.base.*;
import org.apml.base.exceptions.InvalidAPMLFormatException;
import org.apml.converters.ApplicationsConverter;
import com.thoughtworks.xstream.XStream;

/**
 * <p>Responsible for APML-to-Object conversion</p>
 * @author Tim Schultz
 */
public class APMLDeserializer
{
	private XStream xstream = null;
    private ApplicationsConverter appConv = null;
	
	/**
	 * Constructor - Sets up the XStream in accordance to APML specs
	 */
	public APMLDeserializer()
	{
		// Set up the XStream to generate the APML object
		xstream = new XStream();
		appConv = new ApplicationsConverter(xstream.getMapper());
		xstream.alias("APML", APML.class);
		xstream.useAttributeFor(APML.class, "version");
		xstream.useAttributeFor(APML.class, "xmlns");
		xstream.alias("Head", Head.class);
		xstream.alias("Body", Body.class);
		xstream.useAttributeFor(Body.class, "defaultprofile");
		xstream.addImplicitCollection(Body.class, "Profiles");
		xstream.alias("Profile", Profile.class);
		xstream.useAttributeFor(Profile.class, "name");
		xstream.alias("ImplicitData", ImplicitData.class);
		xstream.alias("ExplicitData", ExplicitData.class);
		xstream.alias("Concept", Concept.class);
		xstream.useAttributeFor(Concept.class, "key");
		xstream.useAttributeFor(Concept.class, "value");
		xstream.useAttributeFor(Concept.class, "from");
		xstream.useAttributeFor(Concept.class, "updated");
		xstream.alias("Source", Source.class);
		xstream.useAttributeFor(Source.class, "key");
		xstream.useAttributeFor(Source.class, "name");
		xstream.useAttributeFor(Source.class, "value");
		xstream.useAttributeFor(Source.class, "from");
		xstream.useAttributeFor(Source.class, "updated");
		xstream.alias("Author", Author.class);
		xstream.useAttributeFor(Author.class, "key");
		xstream.useAttributeFor(Author.class, "value");
		xstream.useAttributeFor(Author.class, "from");
		xstream.useAttributeFor(Author.class, "updated");
		xstream.registerConverter(appConv);
		xstream.alias("Applications",Applications.class);
		xstream.alias("Application", Application.class);
		//xstream.alias("Application", Application.class);
		//xstream.useAttributeFor(Application.class, "name");
	}
	
	/**
	 * Deserializes the APML payload to an APML object
	 * @param xmlPayload A string representation of an APML document
	 * @throws InvalidAPMLFormatException
	 * @return The APML object
	 */
	public APML deserialize(String xmlPayload) throws InvalidAPMLFormatException
	{
		return (APML) this.xstream.fromXML(xmlPayload);
	}
	
	/**
	 * Deserializes a file specified by a URL 
	 * @param file The URL pointing to an APML file
	 * @throws IOException
	 * @throws InvalidAPMLFormatException
	 * @return The APML object
	 */
	public APML deserialize(URL file) throws IOException, InvalidAPMLFormatException
	{
		StringBuffer strBuff = null;
		URLConnection connection = null;
		Scanner scanner = null;
		
		strBuff = new StringBuffer();
    	connection = file.openConnection();
    	scanner = new Scanner(connection.getInputStream());
    	scanner.useDelimiter(",\\n*");
    		
    	while(scanner.hasNext())
    		strBuff.append(scanner.next());
    		
    	// Close the connection to the online resource
    	scanner.close();
		return (APML) this.xstream.fromXML(strBuff.toString());
	}
	
	/**
	 * Deserializes a file specified by a File object
	 * @param file A File object pointing to an APML document
	 * @throws FileNotFoundException
	 * @throws IOException
	 * @throws InvalidAPMLFormatException
	 * @return The APML object
	 */
	public APML deserialize(File file) throws FileNotFoundException, IOException, InvalidAPMLFormatException
	{
	    BufferedReader in = null;
	    String lin = "";
	    String payload = "";
	    
		in = new BufferedReader(new FileReader(file));
		
		while ((lin = in.readLine()) != null)
			payload += lin;
		
		return (APML) this.xstream.fromXML(payload);
	}
	
	/**
	 * Returns the XStream for advanced use
	 * @return The XStream object
	 */
	public XStream getXStream()
	{
		return this.xstream;
	}
}
