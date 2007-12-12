package org.apml.deserialize.model;

import java.util.Hashtable;

public class Profiles extends Hashtable
{
	public Profiles()
	{
		super();
	}
	
	public Profile getProfile(String profileName)
	{
		return (Profile) this.get(profileName);
	}
}
