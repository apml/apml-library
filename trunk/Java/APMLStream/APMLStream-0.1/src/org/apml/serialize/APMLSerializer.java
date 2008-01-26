/*	
 * 	APMLStream: Java library for APML serialization/deserialization
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

package org.apml.serialize;

import org.apml.base.*;
import org.apml.converters.ApplicationsConverter;
import com.thoughtworks.xstream.XStream;

/**
 * <p>Responsible for Object-to-APML conversion</p>
 * @author Tim Schultz
 */
public class APMLSerializer
{
	private XStream xstream = null;
	private ApplicationsConverter appConv = null;
	
	/**
	 * Constructor - Sets up the XStream in accordance to APML spec
	 */
	public APMLSerializer()
	{
		xstream = new XStream();
		appConv = new ApplicationsConverter(xstream.getMapper());
		xstream.alias("APML", APML.class);
		xstream.useAttributeFor(APML.class, "version");
		xstream.useAttributeFor(APML.class, "xmlns");
		xstream.alias("Head", Head.class);
		xstream.alias("Body", Body.class);
		xstream.useAttributeFor(org.apml.base.Body.class, "defaultprofile");
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
	 * Serializes an APML object to APML text
	 * @param myAPML
	 * @return The APML markup
	 */
	public String serialize(APML myAPML)
	{
		return this.xstream.toXML(myAPML);
	}
	
	/**
	 * Retrieve the XStream for advanced use
	 * @return XStream
	 */
	public XStream getXStream()
	{
		return this.xstream;
	}
}
