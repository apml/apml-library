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

package org.apml.tests;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.net.URL;

import org.apml.base.APML;
import org.apml.base.ExplicitData;
import org.apml.base.ImplicitData;
import org.apml.base.Profile;
import org.apml.deserialize.APMLDeserializer;
import org.apml.serialize.APMLSerializer;
import org.apml.tools.APMLTools;

/**
 * <p>Tests and demos the functionality of the APML serializer and deserializer.</p>
 * @author Tim Schultz
 */
public class APMLDeserializeTest
{

	/**
	 * Simple test to demo and show features
	 * @param args
	 */
	public static void main(String[] args)
	{
	    File f = null;
	    BufferedReader in = null;
	    APMLDeserializer apmlDS = null;
	    APMLSerializer apmlS = null;
	    String lin = "";
	    String payload = "";
	    
		try
		{
			f = new File(args[0]);
			in = new BufferedReader(new FileReader(f));

			// Read in the .apml file
			while ((lin = in.readLine()) != null)
				payload += lin;
			
			// Create the APML object from the file contents
			apmlDS = new APMLDeserializer();
			APML myAPML = apmlDS.deserialize(payload);
			//APML myAPML = apmlDS.deserialize(new URL("http://aura.darkstar.sunlabs.com/AttentionProfile/apml/last.fm/phocion55"));
			
			// ***********************************************
			// *** Demonstrates how to use the APML object ***
			// Get the APML head info
			System.out.println(	myAPML.getHead().getTitle() + ", " +
								myAPML.getHead().getDateCreated() + ", " +
								myAPML.getHead().getUserEmail() + ", " + 
								myAPML.getHead().getGenerator());
			
			// Get the APML body info
			System.out.println(myAPML.getBody().getDefaultProfileName());
			
			// Get a specific profile
			Profile p = myAPML.getBody().getProfileByName("overall-music");
			// Or get a default profile
			Profile defaultP = myAPML.getBody().getDefaultProfile();
			System.out.println(p.getName());
			System.out.println(defaultP.getName());
			
			// Get implicit and explicit data from the profile
			ImplicitData iData = p.getImplicitData();
			ExplicitData eData = p.getExplicitData();
			// *************************************************
			// *************************************************

			// Convert back to APML
			apmlS = new APMLSerializer();
			System.out.println(apmlS.serialize(myAPML));
		}
		
		catch(Exception ex)
		{
			ex.printStackTrace();
		}
	}

}
