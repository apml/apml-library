package org.apml.deserialize.model;

public class Profile
{
	private String name = "";
	private ImplicitData implicitData = null;
	private ExplicitData explicitData = null;
	
	public Profile(){}
	
	public Profile(String profileName)
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
}
