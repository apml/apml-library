/*	
 *  APMLStream: Java library for APML serialization/deserialization
 *	By:  Tim Schultz
 *
 *	Special thanks to Jörg Schaible of XStream!
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

package org.apml.converters;

import java.util.Iterator;
import org.apml.base.Application;
import org.apml.base.Applications;
import com.thoughtworks.xstream.converters.Converter;
import com.thoughtworks.xstream.converters.MarshallingContext;
import com.thoughtworks.xstream.converters.UnmarshallingContext;
import com.thoughtworks.xstream.io.HierarchicalStreamReader;
import com.thoughtworks.xstream.io.HierarchicalStreamWriter;
import com.thoughtworks.xstream.mapper.Mapper;

/**
 * <p>We can't directly serialize/deserialize Application tags, because we don't really know what to expect.<br>
 * We have to register an Application tag converter and iterate through its nodes manually.
 * </p>
 * @author Tim Schultz
 */
public class ApplicationsConverter implements Converter
{
	private Mapper map = null;
	
	/**
	 * Default constructor
	 * @param map The XStream mapper
	 */
	public ApplicationsConverter(Mapper map)
	{
		this.map = map;
	}
	
	/**
	 * Marshalls (Object-to-XML) the object
	 * @param obj The object you wish to marshall (canConvert MUST return true)
	 * @param hStrWrite The HierarchicalStreamWriter used to write the XML nodes
	 * @param mContext The MarshallingContext - currently not implemented
	 */
	public void marshal(Object obj, HierarchicalStreamWriter hStrWrite, MarshallingContext mContext)
	{
		// TODO Write the XML properly
		Applications<Application> apps = (Applications<Application>) obj;
		Iterator<Application> i = apps.iterator();
		Application currApp = null;
		
		while(i.hasNext())
		{
			currApp = i.next();
			hStrWrite.startNode("Application");
			hStrWrite.addAttribute("name", currApp.getName());
			hStrWrite.setValue("\n        " + currApp.getApplicationData() + "\n      ");
			hStrWrite.endNode();
		}
	}

	/**
	 * Unmarshalls (XML-to-Object) the payload
	 * @param hStrRead The HierarchicalStreamReader
	 * @param umContext UnmarshallingContext - currently not implemented
	 */
	public Object unmarshal(HierarchicalStreamReader hStrRead, UnmarshallingContext umContext)
	{
		// TODO Currently doesn't capture all data
		Applications<Application> apps  = new Applications();
		Application currApp = null;
        String currNodeName = "";
        String currNodeValue = "";
        
        while (hStrRead.hasMoreChildren())
        {
        	hStrRead.moveDown();
            currNodeName = hStrRead.getNodeName().trim();
            System.out.println(currNodeName);
            currApp = new Application(hStrRead.getAttribute("name"), "payload_goes_here");
            apps.add(currApp);
            hStrRead.moveUp();
        }
		return apps;
	}

	/**
	 * Determines whether a specific class can be converted through this specific converter.
	 * @param cls The class name
	 */
	public boolean canConvert(Class cls)
	{
		return cls.getName().equals("org.apml.base.Applications");
	}
}
