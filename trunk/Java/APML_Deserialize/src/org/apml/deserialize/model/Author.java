package org.apml.deserialize.model;

public class Author
{
	private String key = "";
	private String value = "";
	private String from = "";
	private String updated = "";
	
	public Author(){}
	
	public Author(String key, String value, String from, String updated)
	{
		this.key = key;
		this.value = value;
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
}