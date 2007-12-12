package org.apml.deserialize.model;

public class Source
{
	private String key = "";
	private String value = "";
	private String type = "";
	private String from = "";
	private String updated = "";
	private Author author = null;
	
	public Source(){}
	
	public Source(String key, String value, String type, String from, String updated)
	{
		this.key = key;
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
}
