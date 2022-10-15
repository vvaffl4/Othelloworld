import { configureStore } from '@reduxjs/toolkit'
import * as Api from '../api';
import ui from './UserInterface';
import nav from './Nav';
import world from './World';
import auth from './Auth';
import games from './Games';
import game from './Game';
import news from './News';

const authState = sessionStorage.getItem('auth')
  ? JSON.parse(sessionStorage.getItem('auth')!)
  : {}

const store = configureStore({
  reducer: {
    ui,
    nav,
    world,
    auth,
    games,
    game,
    news
  },
  preloadedState: {
    auth: authState
	},
  middleware: getDefaultMiddleware =>
    getDefaultMiddleware({
      thunk: {
        extraArgument: Api
      }
    })
})

store.subscribe(() => {
  sessionStorage.setItem('auth', JSON.stringify(store.getState().auth));
})

export default store;

export type ApiRequest = (dispatch: AppDispatch, getState: () => RootState, api: typeof Api) => void
// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>
// Inferred type: {posts: PostsState, comments: CommentsState, users: UsersState}
export type AppDispatch = typeof store.dispatch