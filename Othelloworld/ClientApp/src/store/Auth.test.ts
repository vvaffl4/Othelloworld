import Token from '../model/Token';
import reducer, { AuthState, setToken, unsetToken } from './Auth';

const defaultState: AuthState = {
    authenticated: false,
    token: '',
    expires: ''
}

describe('Bla', () => {
  test('Set token', () => {
    const token: Token = {
      token: 'Bearer Token',
      player: { username: 'TokenUser', amountWon: 0, amountDraw: 0, amountLost: 0, countryCode: 'NL' },
      expires: 'Some day'
    }

    expect(reducer(defaultState, setToken(token)))
      .toEqual({
        ...token,
        authenticated: true
      });
	})
});