package org.apml.deserialize.model;

public class DateCreated
{
	private String dateCreated = "";
	
	// TODO Check for valid date format
	public DateCreated(){}
	
	public DateCreated(String dateCreated)
	{
		this.dateCreated = dateCreated;
	}

	public String getDateCreated() {
		return dateCreated;
	}

	public void setDateCreated(String dateCreated) {
		this.dateCreated = dateCreated;
	}
}
