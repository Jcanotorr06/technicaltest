import { type Configuration, LogLevel } from "@azure/msal-browser";

const msalConfig = {
	auth: {
		clientId: import.meta.env.VITE_CLIENT_ID,
		authority: import.meta.env.VITE_CLIENT_AUTHORITY,
		redirectUri: import.meta.env.VITE_REDIRECT_URI,
		postLogoutRedirectUri: import.meta.env.VITE_LOGOUT_URI,
		navigateToLoginRequestUrl: false,
	},
	cache: {
		cacheLocation: "sessionStorage",
	},
	system: {
		loggerOptions: {
			loggerCallback: (level, message, containsPii) => {
				if (containsPii) return;

				switch (level) {
					case LogLevel.Error:
						console.error(message);
						break;
					case LogLevel.Info:
						console.info(message);
						break;
					case LogLevel.Verbose:
						console.debug(message);
						break;
					case LogLevel.Warning:
						console.warn(message);
						break;
					default:
						console.log(message);
				}
			},
		},
	},
} satisfies Configuration;

const loginRequest = {
	scopes: [],
};

export { msalConfig, loginRequest };
