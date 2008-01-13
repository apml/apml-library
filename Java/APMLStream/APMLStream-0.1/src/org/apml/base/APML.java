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
 * <p>The Java implementation of the &lt;APML&gt; tag.</p>
 * @author Tim Schultz
 */
public class APML
{
	private String xmlns = "";
	private String version = "";
	private Head Head = null;
	private Body Body = null;
	
	public APML(){}
	
	/**
	 * Constructor
	 * @param version The specified APML spec version
	 */
	public APML(String version) {
		this.version = version;
	}

	public String getVersion() {
		return version;
	}

	public void setVersion(String version) {
		this.version = version;
	}

	public Head getHead() {
		return Head;
	}

	public void setHead(Head head) {
		this.Head = head;
	}

	public Body getBody() {
		return Body;
	}

	public void setBody(Body body) {
		this.Body = body;
	}

	public String getXmlns() {
		return xmlns;
	}

	public void setXmlns(String xmlns) {
		this.xmlns = xmlns;
	}
}
