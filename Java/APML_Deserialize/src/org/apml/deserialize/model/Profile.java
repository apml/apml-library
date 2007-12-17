package org.apml.deserialize.model;

import java.util.ArrayList;
import java.util.Iterator;

public class Profile
{
	private String name = "";
	private ImplicitData implicitData = null;
	private ExplicitData explicitData = null;
	
	public Profile(){}
	
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
		return implicitData;
	}

	public void setImplicitData(ImplicitData implicitData) {
		this.implicitData = implicitData;
	}

	public ExplicitData getExplicitData() {
		return explicitData;
	}

	public void setExplicitData(ExplicitData explicitData) {
		this.explicitData = explicitData;
	}
	
	public ArrayList getAllImplicitConceptsFrom(String from)
	{
		ArrayList fromList = new ArrayList();
		Iterator iIConcept = this.implicitData.getConcepts().iterator();
		while(iIConcept.hasNext())
		{
			Concept currCon = (Concept) iIConcept.next();
			if(currCon.getFrom().equals(from))
				fromList.add(currCon);
		}
		return fromList;
	}
	
	public ArrayList getAllExplicitConceptsFrom(String from)
	{
		ArrayList fromList = new ArrayList();
		Iterator iIConcept = this.explicitData.getConcepts().iterator();
		while(iIConcept.hasNext())
		{
			Concept currCon = (Concept) iIConcept.next();
			if(currCon.getFrom().equals(from))
				fromList.add(currCon);
		}
		return fromList;
	}
}
