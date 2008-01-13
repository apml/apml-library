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

import java.io.File;
import java.net.URL;
import org.apml.base.APML;
import com.thoughtworks.xstream.XStream;

/**
 * <p>Responsible for APML-to-Object conversion</p>
 * @author Tim Schultz
 */
public class APMLDeserializer
{
	private XStream xstream = null;
	
	/**
	 * Constructor - Sets up the XStream in accordance to APML specs
	 */
	public APMLDeserializer()
	{
		// Set up the XStream to generate the APML object
		xstream = new XStream();
		xstream.alias("APML", org.apml.base.APML.class);
		xstream.useAttributeFor(org.apml.base.APML.class, "version");
		xstream.useAttributeFor(org.apml.base.APML.class, "xmlns");
		xstream.alias("Head", org.apml.base.Head.class);
		xstream.alias("Body", org.apml.base.Body.class);
		xstream.useAttributeFor(org.apml.base.Body.class, "defaultprofile");
		xstream.addImplicitCollection(org.apml.base.Body.class, "Profiles");
		xstream.alias("Profile", org.apml.base.Profile.class);
		xstream.useAttributeFor(org.apml.base.Profile.class, "name");
		xstream.alias("ImplicitData", org.apml.base.ImplicitData.class);
		xstream.alias("ExplicitData", org.apml.base.ExplicitData.class);
		xstream.alias("Concept", org.apml.base.Concept.class);
		xstream.useAttributeFor(org.apml.base.Concept.class, "key");
		xstream.useAttributeFor(org.apml.base.Concept.class, "value");
		xstream.useAttributeFor(org.apml.base.Concept.class, "from");
		xstream.useAttributeFor(org.apml.base.Concept.class, "updated");
		xstream.alias("Source", org.apml.base.Source.class);
		xstream.useAttributeFor(org.apml.base.Source.class, "key");
		xstream.useAttributeFor(org.apml.base.Source.class, "name");
		xstream.useAttributeFor(org.apml.base.Source.class, "value");
		xstream.useAttributeFor(org.apml.base.Source.class, "from");
		xstream.useAttributeFor(org.apml.base.Source.class, "updated");
		xstream.alias("Author", org.apml.base.Author.class);
		xstream.useAttributeFor(org.apml.base.Author.class, "key");
		xstream.useAttributeFor(org.apml.base.Author.class, "value");
		xstream.useAttributeFor(org.apml.base.Author.class, "from");
		xstream.useAttributeFor(org.apml.base.Author.class, "updated");
		xstream.alias("Application", org.apml.base.Application.class);
		xstream.useAttributeFor(org.apml.base.Application.class, "name");
	}
	
	/**
	 * Deserializes the APML payload to an APML object
	 * @param xmlPayload
	 * @return The APML object
	 */
	public APML deserialize(String xmlPayload)
	{
		return (APML) this.xstream.fromXML(xmlPayload);
	}
	
	/**
	 * Deserializes a file specified by a URL 
	 * @param file
	 * @return The APML object
	 */
	public APML deserialize(URL file)
	{
		// TODO
		return (APML) this.xstream.fromXML("");
	}
	
	/**
	 * Deserializes a file specified by a File object
	 * @param file
	 * @return The APML object
	 */
	public APML deserialize(File file)
	{
		// TODO
		return (APML) this.xstream.fromXML("");
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
