package org.apml.deserialize.model;

public class Head
{
	private Title title = null;
	private Generator generator = null;
	private UserEmail userEmail = null;
	private DateCreated dateCreated = null;
	
	public Head(){}

	public Title getTitle() {
		return title;
	}

	public void setTitle(Title title) {
		this.title = title;
	}

	public Generator getGenerator() {
		return generator;
	}

	public void setGenerator(Generator generator) {
		this.generator = generator;
	}

	public UserEmail getUserEmail() {
		return userEmail;
	}

	public void setUserEmail(UserEmail userEmail) {
		this.userEmail = userEmail;
	}

	public DateCreated getDateCreated() {
		return dateCreated;
	}

	public void setDateCreated(DateCreated dateCreated) {
		this.dateCreated = dateCreated;
	}
}
