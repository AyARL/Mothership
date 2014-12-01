<?php
	abstract class Response
	{
		const NoError = 0;
		const Error_InvalidHash = 1;
	}

	abstract class AccountCreationResponse extends Response
	{
		const Error_EmailEmpty = 2;
		const Error_EmailInUse = 3;
		const Error_PasswordEmpty = 4;
		const Error_NameInUse = 5;
	}

	abstract class LoginResponse extends Response
	{
		const Error_IncorrectCredentials = 2;
	}

	abstract class GetProfileResponse extends Response
	{
		const Error_IncorrectCredentials = 2;
		const Error_NoProfile = 3;
	}

	abstract class GetLastGameResponse extends Response
	{
		const Error_NoData = 2;
	}
?>