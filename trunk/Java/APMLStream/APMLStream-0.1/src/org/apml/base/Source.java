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
 * <p>The Java implementation of the &lt;Source&gt; tag</p>
 * @author Tim Schultz
 */
public class Source
{
	private String key = "";
	private String name = "";
	private String value = "";
	private String type = "";
	private String from = "";
	private String updated = "";
	private Author author = null;
	
	public Source(){}
	
	/**
	 * Constructor
	 * @param key The key attribute
	 * @param name The name attribute
	 * @param value	The value attribute
	 * @param type The type attribute
	 * @param from The from attribute
	 * @param updated The updated attribute
	 */
	public Source(String key, String name, String value, String type, String from, String updated)
	{
		this.key = key;
		this.name = name;
		this.value = value;
		this.type = type;
		this.from = from;
		this.updated = updated;
	}
	
	public String getKey() {
		return key;
	}
	public void setKey(String key) {
		this.key = key;
	}
	public String getValue() {
		return value;
	}
	public void setValue(String value) {
		this.value = value;
	}
	public String getType() {
		return type;
	}
	public void setType(String type) {
		this.type = type;
	}
	public String getFrom() {
		return from;
	}
	public void setFrom(String from) {
		this.from = from;
	}
	public String getUpdated() {
		return updated;
	}
	public void setUpdated(String updated) {
		this.updated = updated;
	}

	public Author getAuthor() {
		return author;
	}

	public void setAuthor(Author author) {
		this.author = author;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}
}
