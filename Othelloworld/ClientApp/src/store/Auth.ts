import Token from "../model/Token";
import { createSlice, Dispatch, PayloadAction } from "@reduxjs/toolkit";
import { ApiRequest } from ".";
import { AccountDTO, ErrorResponse, LoginDTO } from "../api";

export interface AuthState extends Token {
	authenticated: boolean
}

const initialState: AuthState = {
	token: '',
	username: '',
	expires: '',
	authenticated: false
};

const authSlice = createSlice({
	name: 'auth',
	initialState,
	reducers: {
		setToken: (state, action: PayloadAction<Token>) => {
			state.token = action.payload.token;
			state.username = action.payload.username;
			state.expires = action.payload.expires;
			state.authenticated = true;
		},
		unsetToken: (state) => {
			state.token = "";
			state.expires = "";
			state.authenticated = false;
		}
	}
});

export const register = (account: AccountDTO, errorCallback: ErrorResponse): ApiRequest =>
	async (dispatch, _, { register }) =>
		register(account)
			.then(token => dispatch(authSlice.actions.setToken(token)))
			.catch(errorCallback);
export const login = (credentials: LoginDTO, errorCallback: ErrorResponse): ApiRequest =>
	async (dispatch, _, { login }) =>
		login(credentials)
			.then(token => dispatch(authSlice.actions.setToken(token)))
			.catch(errorCallback);
export const logout = (): ApiRequest =>
	async (dispatch, getState, { logout }) =>
		logout(getState().auth)
			.then(_ => dispatch(authSlice.actions.unsetToken()))

export const { setToken, unsetToken } = authSlice.actions;

export default authSlice.reducer;