package org.apml.deserialize.model;

public class UserEmail
{
	private String userEmail = "";
	
	public UserEmail(){}
	
	public UserEmail(String userEmail)
	{
		this.userEmail = userEmail;
	}

	public String getUserEmail() {
		return userEmail;
	}

	public void setUserEmail(String userEmail) {
		this.userEmail = userEmail;
	}
}
