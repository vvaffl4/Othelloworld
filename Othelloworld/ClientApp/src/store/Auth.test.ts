import Token from '../model/Token';
import reducer, { AuthState, setToken, unsetToken } from './Auth';

const defaultState: AuthState = {
    authenticated: false,
    token: '',
    username: '',
    expires: ''
}

describe('Bla', () => {
  test('Set token', () => {
    const token: Token = {
        token: 'Bearer Token',
        username: 'TokenUser',
        expires: 'Some day'
    }

    expect(reducer(defaultState, setToken(token)))
      .toEqual({
        ...token,
        authenticated: true
      });
	})
});