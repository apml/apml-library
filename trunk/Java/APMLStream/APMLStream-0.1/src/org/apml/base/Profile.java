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

/**
 * <p>The Java implementation of the &lt;Profile&gt; tag.</p>
 * @author Tim Schultz
 */
public class Profile
{
	private String name = "";
	private ImplicitData ImplicitData = null;
	private ExplicitData ExplicitData = null;
	
	public Profile(){}
	
	/**
	 * Constructor
	 * @param name The name of the profile
	 */
	public Profile(String name)
	{
		this.name = name;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public ImplicitData getImplicitData() {
		return ImplicitData;
	}

	public void setImplicitData(ImplicitData implicitData) {
		this.ImplicitData = implicitData;
	}

	public ExplicitData getExplicitData() {
		return ExplicitData;
	}

	public void setExplicitData(ExplicitData explicitData) {
		this.ExplicitData = explicitData;
	}
}
