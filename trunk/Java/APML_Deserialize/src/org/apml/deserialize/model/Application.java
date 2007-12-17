package org.apml.deserialize.model;

public class Application
{
	private String name = "";
	private String applicationData = "";
	
	public Application(){}
	
	public Application(String name, String applicationData)
	{
		this.name = name;
		this.applicationData = applicationData;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public String getApplicationData() {
		return applicationData;
	}

	public void setApplicationData(String applicationData) {
		this.applicationData = applicationData;
	}
}
