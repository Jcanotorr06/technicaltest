import { msalConfig } from "@/config";
import { PublicClientApplication } from "@azure/msal-browser";
import { MsalProvider } from "@azure/msal-react";
import type { FC, PropsWithChildren } from "react";

const msalInstance = new PublicClientApplication(msalConfig);

if (
	!msalInstance.getActiveAccount() &&
	msalInstance.getAllAccounts().length > 0
) {
	msalInstance.setActiveAccount(msalInstance.getAllAccounts()[0]);
}

/* msalInstance.addEventCallback((event) => {
  if (event.eventType === EventType.LOGIN_SUCCESS && event.payload) {
    const account = Object.hasOwn(event.payload, "homeAccountId")
      ? (event.payload as AccountInfo)
      : null;
    msalInstance.setActiveAccount(account);
  }
}); */

const AuthProvider: FC<PropsWithChildren> = ({ children }) => {
	return <MsalProvider instance={msalInstance}>{children}</MsalProvider>;
};

AuthProvider.displayName = "AuthProvider";

export default AuthProvider;
