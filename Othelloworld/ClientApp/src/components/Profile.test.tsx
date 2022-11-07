// react-testing-library renders your components to document.body,
// this adds jest-dom's custom assertions
import '@testing-library/jest-dom';

import { render, screen, waitFor } from '@testing-library/react';
import React from 'react';
import * as ReactRouterHooks from 'react-router/lib/hooks';
import Player from '../model/Player'
import * as Hooks from '../store/Hooks';
import * as Api from '../api';
import Profile from './Profile'



describe("profile", () => {
	test("username", async () => {
		jest.mock('react-router-dom', () => ({
			...jest.requireActual('react-router-dom'), // use actual for all non-hook parts
			useParams: () => ({
				username: 'profileUsername'
			}),
		}));

		const getPlayerMock = (token, username) => new Promise((resolve) => {
			resolve({
				username,
				amountWon: 0,
				amountDraw: 1,
				amountLost: 2
			} as Player);
		});

		//var getPlayerSpy = jest.spyOn(Api, 'getPlayer')
		//	.mockImplementation(getPlayerMock);

		jest.spyOn(Hooks, 'useAppSelector')
			.mockReturnValue({
				token: 'test'
			});

		render(<Profile />);

		await waitFor(() => {
			expect(getPlayerMock).toBeCalled();
			expect(screen.queryByText('profileUsername')).not.toBeNull();
			expect(screen.queryByText('0')).not.toBeNull();
			expect(screen.queryByText('1')).not.toBeNull();
			expect(screen.queryByText('2')).not.toBeNull();
		}, { interval: 1000, timeout: 3500 });
	});
});