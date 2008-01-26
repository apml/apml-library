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

package org.apml.base;

import java.util.ArrayList;
import java.util.List;
import org.apml.base.Applications;
import org.apml.base.exceptions.ProfileNotFoundException;

/**
 * <p>The Java implementation of the &lt;Body&gt; tag.</p>
 * @author Tim Schultz
 */
public class Body
{
	private String defaultprofile = "";
	private Applications<Application> Applications = null;
	private List<Profile> Profiles = null;
	
	public Body(){}
	
	/**
	 * Constructor
	 * @param defaultProfile The defaultprofile attribute
	 */
	public Body(String defaultProfile)
	{
		this.defaultprofile = defaultProfile;
		this.Profiles = new ArrayList();
		this.Applications = new Applications();
	}

	public String getDefaultProfileName() {
		return defaultprofile;
	}

	public void setDefaultProfileName(String defaultProfile) {
		this.defaultprofile = defaultProfile;
	}

	public List<Application> getApplications() {
		return this.Applications;
	}

	public void setApplications(Applications<Application> applications) {
		this.Applications = applications;
	}

	public List<Profile> getProfiles() {
		return Profiles;
	}

	public void setProfiles(List<Profile> profiles) {
		this.Profiles = profiles;
	}
	
	/**
	 * Retrieve a profile by name
	 * @param name The profile name
	 * @return The profile
	 * @throws ProfileNotFoundException
	 */
	public Profile getProfileByName(String name) throws ProfileNotFoundException
	{
		int i = 0;
		for(i = 0; i < this.Profiles.size(); i++)
		{
			if(this.Profiles.get(i).getName().equalsIgnoreCase(name))
				return this.Profiles.get(i);
		}
		 throw new ProfileNotFoundException("The profile '" + name + "' does not exist!");
	}
	
	/**
	 * Retrieve the profile specified as the defaultprofile
	 * @return The profile
	 * @throws ProfileNotFoundException
	 */
	public Profile getDefaultProfile() throws ProfileNotFoundException
	{
		int i = 0;
		for(i = 0; i < this.Profiles.size(); i++)
		{
			if(this.Profiles.get(i).getName().equalsIgnoreCase(this.defaultprofile))
				return this.Profiles.get(i);
		}
		throw new ProfileNotFoundException("The default profile '" + this.defaultprofile + "' does not exist!");
	}
}
