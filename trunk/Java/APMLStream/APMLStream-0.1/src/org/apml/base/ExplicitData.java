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

package org.apml.base;

import java.util.ArrayList;
import java.util.List;

import org.apml.tools.APMLTools;

/**
 * <p>The Java implementation of the &lt;ExplicitData&gt; tag.</p>
 * @author Tim Schultz
 */
public class ExplicitData
{
	private List<Concept> Concepts = null;
	private List<Source> Sources = null;
	
	public ExplicitData()
	{
		Concepts = new ArrayList();
		Sources = new ArrayList();
	}
	
	public List<Concept> getConcepts() {
		return Concepts;
	}
	public void setConcepts(List<Concept> concepts) {
		this.Concepts = concepts;
	}
	public List<Source> getSources() {
		return Sources;
	}
	public void setSources(List<Source> sources) {
		this.Sources = sources;
	}
	
	/**
	 * Retrieve all concepts from a specific resource
	 * @param from The resource specified in the 'from' attribute
	 * @return A list of all matching concepts
	 */
	public ArrayList<Concept> getAllConceptsFrom(String from)
	{
		int i = 0;
		ArrayList<Concept> sublist = new ArrayList();
		
		for(i = 0; i < this.Concepts.size(); i++)
			if(this.Concepts.get(i).getFrom().equalsIgnoreCase(from))
				sublist.add(this.Concepts.get(i));
		
		return sublist;
	}
	
	/**
	 * Retrieve all sources from a specific resource
	 * @param from The resource specified in the 'from' attribute
	 * @return A list of all matching sources
	 */
	public ArrayList<Source> getAllSourcesFrom(String from)
	{
		int i = 0;
		ArrayList<Source> sublist = new ArrayList();
		
		for(i = 0; i < this.Sources.size(); i++)
			if(this.Sources.get(i).getFrom().equalsIgnoreCase(from))
				sublist.add(this.Sources.get(i));
		
		return sublist;
	}
	
	/**
	 * Retrieve all concepts that are outdated
	 * @return A list of outdated concepts
	 */
	public ArrayList<Concept> getAllOutdatedConcepts()
	{
		int i = 0;
		ArrayList<Concept> sublist = new ArrayList();
		
		for(i = 0; i < this.Concepts.size(); i++)
			if(APMLTools.isResourceOutdated(this.Concepts.get(i).getUpdated()))
				sublist.add(this.Concepts.get(i));
		
		return sublist;
	}
	
	/**
	 * Retrieve all sources that are outdated
	 * @return A list of outdated sources
	 */
	public ArrayList<Source> getAllOutdatedSources()
	{
		int i = 0;
		ArrayList<Source> sublist = new ArrayList();
		
		for(i = 0; i < this.Sources.size(); i++)
			if(APMLTools.isResourceOutdated(this.Sources.get(i).getUpdated()))
				sublist.add(this.Sources.get(i));
		return sublist;
	}
}
