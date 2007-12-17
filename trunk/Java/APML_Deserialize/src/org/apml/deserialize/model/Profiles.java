package org.apml.deserialize.model;

import java.util.Hashtable;

import org.apml.deserialize.exceptions.ProfileDoesNotExistException;

public class Profiles extends Hashtable
{
	public Profiles()
	{
		super();
	}
	
	public Profile getProfile(String profileName) throws ProfileDoesNotExistException
	{
		if(this.get(profileName) == null)
			throw new ProfileDoesNotExistException("The Profile " + profileName + " does not exist!");
		else
			return (Profile) this.get(profileName);
	}
}
