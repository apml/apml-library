package org.apml.deserialize.model;

public class Concept
{
	private String key = "";
	private String value = "";
	private String uri = "";
	private String from = "";
	private String updated = "";
	
	public Concept(){}
	
	public Concept(String key, String value, String uri, String from, String updated)
	{
		this.key = key;
		this.value = value;
		this.uri = uri;
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
	
	public String getURI()
	{
		return this.uri;
	}
	
	public void setURI(String uri)
	{
		this.uri = uri;
	}
}
