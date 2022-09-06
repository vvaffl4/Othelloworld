import Token from "../model/Token";
import { createSlice, Dispatch, PayloadAction } from "@reduxjs/toolkit";
import { ApiRequest } from ".";

interface AuthState extends Token {
	authenticated: boolean
}

const initialState: AuthState = {
	token: '',
	expires: '',
	authenticated: false
};

const authSlice = createSlice({
	name: 'auth',
	initialState,
	reducers: {
		setToken: (state, action: PayloadAction<{ token: string, expires: string }>) => {
			state.token = action.payload.token;
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

export const register = (username: string, email: string, password: string): ApiRequest => async (dispatch, _, { register }) =>
	register(username, email, password)
		.then(token => dispatch(authSlice.actions.setToken(token)));
export const login = (identifier: string, password: string): ApiRequest => async (dispatch, _, { login }) =>
	login(identifier, password)
		.then(token => dispatch(authSlice.actions.setToken(token)));
export const logout = () => (dispatch: Dispatch) =>
	dispatch(authSlice.actions.unsetToken())

export default authSlice.reducer;