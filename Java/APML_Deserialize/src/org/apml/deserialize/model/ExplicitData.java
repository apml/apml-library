package org.apml.deserialize.model;

public class ExplicitData
{
	private Concepts concepts = null;
	private Sources sources = null;
	
	public ExplicitData()
	{
		concepts = new Concepts();
		sources = new Sources();
	}
	
	public Concepts getConcepts() {
		return concepts;
	}
	public void setConcepts(Concepts concepts) {
		this.concepts = concepts;
	}
	public Sources getSources() {
		return sources;
	}
	public void setSources(Sources sources) {
		this.sources = sources;
	}
}
