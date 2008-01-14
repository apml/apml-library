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
 * <p>The Java implementation of the &lt;Head&gt; tag</p>
 * @author Tim Schultz
 */
public class Head
{
	private String Title = null;
	private String Generator = null;
	private String UserEmail = null;
	private String DateCreated = null;
	
	public Head(){}
	
	/**
	 * Constructor
	 * @param title The title of the APML list
	 * @param generator The name of the generating application
	 * @param userEmail The user email address
	 * @param dateCreated The date created
	 */
	public Head(String title, String generator, String userEmail, String dateCreated)
	{
		this.Title = title;
		this.Generator = generator;
		this.UserEmail = userEmail;
		this.DateCreated = dateCreated;
	}

	public String getTitle() {
		return Title;
	}

	public void setTitle(String title) {
		this.Title = title;
	}

	public String getGenerator() {
		return Generator;
	}

	public void setGenerator(String generator) {
		this.Generator = generator;
	}

	public String getUserEmail() {
		return UserEmail;
	}

	public void setUserEmail(String userEmail) {
		this.UserEmail = userEmail;
	}

	public String getDateCreated() {
		return DateCreated;
	}

	public void setDateCreated(String dateCreated) {
		this.DateCreated = dateCreated;
	}
}
