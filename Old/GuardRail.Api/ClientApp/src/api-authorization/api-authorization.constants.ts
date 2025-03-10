export const ApplicationName = 'GuardRail';

export const ReturnUrlType = 'returnUrl';

export const QueryParameterNames = {
  ReturnUrl: ReturnUrlType,
  Message: 'message'
};

export const LogoutActions = {
  LogoutCallback: 'logout-callback',
  Logout: 'logout',
  LoggedOut: 'logged-out',
  ResetPassword: 'reset-password'
};

export const LoginActions = {
  Login: 'login',
  LoginCallback: 'login-callback',
  LoginFailed: 'login-failed'
};

let applicationPaths: IApplicationPathsType = {
  DefaultLoginRedirectPath: '/',
  Login: `authentication/${LoginActions.Login}`,
  LoginFailed: `authentication/${LoginActions.LoginFailed}`,
  LoginCallback: `authentication/${LoginActions.LoginCallback}`,
  LogOut: `authentication/${LogoutActions.Logout}`,
  LoggedOut: `authentication/${LogoutActions.LoggedOut}`,
  LogOutCallback: `authentication/${LogoutActions.LogoutCallback}`,
  ResetPassword: `authentication/${LogoutActions.ResetPassword}`,
  LoginPathComponents: [],
  LoginFailedPathComponents: [],
  LoginCallbackPathComponents: [],
  LogOutPathComponents: [],
  LoggedOutPathComponents: [],
  LogOutCallbackPathComponents: []
};

applicationPaths = {
  ...applicationPaths,
  LoginPathComponents: applicationPaths.Login.split('/'),
  LoginFailedPathComponents: applicationPaths.LoginFailed.split('/'),
  LogOutPathComponents: applicationPaths.LogOut.split('/'),
  LoggedOutPathComponents: applicationPaths.LoggedOut.split('/'),
  LogOutCallbackPathComponents: applicationPaths.LogOutCallback.split('/')
};

interface IApplicationPathsType {
  readonly DefaultLoginRedirectPath: string;
  readonly Login: string;
  readonly LoginFailed: string;
  readonly LoginCallback: string;
  readonly LogOut: string;
  readonly LoggedOut: string;
  readonly LogOutCallback: string;
  readonly ResetPassword: string;
  readonly LoginPathComponents: string [];
  readonly LoginFailedPathComponents: string [];
  readonly LoginCallbackPathComponents: string [];
  readonly LogOutPathComponents: string [];
  readonly LoggedOutPathComponents: string [];
  readonly LogOutCallbackPathComponents: string [];
}

export const ApplicationPaths = applicationPaths;
