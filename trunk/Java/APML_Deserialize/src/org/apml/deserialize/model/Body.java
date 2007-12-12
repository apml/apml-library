package org.apml.deserialize.model;

import java.util.Hashtable;

public class Body
{
	private String defaultProfile = "";
	private Applications applications = null;
	private Profiles profiles;
	
	public Body(){}
	
	public Body(String defaultProfile)
	{
		this.defaultProfile = defaultProfile;
		this.profiles = new Profiles();
	}

	public String getDefaultProfile() {
		return defaultProfile;
	}

	public void setDefaultProfile(String defaultProfile) {
		this.defaultProfile = defaultProfile;
	}

	public Applications getApplications() {
		return applications;
	}

	public void setApplications(Applications applications) {
		this.applications = applications;
	}

	public Profiles getProfiles() {
		return profiles;
	}

	public void setProfiles(Profiles profiles) {
		this.profiles = profiles;
	}
}
