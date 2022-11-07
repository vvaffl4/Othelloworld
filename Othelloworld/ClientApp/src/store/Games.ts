import { Table } from "@mui/material"
import { createSlice, PayloadAction } from "@reduxjs/toolkit"
import { ApiRequest } from "."
import Game from "../model/Game"
import PagedList from "../model/PagedList"


export type Library<T extends number | string, U> = {
  [i in T]: U
}

export type Table<T extends number | string, U> = {
  byId: Library<T, U>,
  allIds: T[]
}

export interface GamesState extends PagedList<Game> { }

const initialState: GamesState = {
  currentPage: 1,
  totalPages: 0,
  pageSize: 5,
  totalCount: 0,
  items: []
}

const gamesSlice = createSlice({
  name: 'games',
  initialState,
  reducers: {
    addGames: (state, { payload }: PayloadAction<PagedList<Game>>) => {
      state.currentPage = payload.currentPage;
      state.totalPages = payload.totalPages;
      state.pageSize = payload.pageSize;
      state.totalCount = payload.totalCount;
      state.items = payload.items;
      //state.byId = {
      //  ...state.byId,
      //  ...action.payload.reduce((byId, game) => ({ ...byId, [game.token]: game }), {})
      //}
      //state.allIds = [...new Set([
      //  ...state.allIds,
      //  ...action.payload.map(game => game.token)
      //])]
    }
  },
})

// Thunks
export const fetchGames = (pageNumber: number, pageSize: number): ApiRequest =>
  async (dispatch, getState, { getGames }) =>
    getGames(pageNumber, pageSize, getState().auth)
      .then(games => dispatch(addGames(games)));
export const searchGames = (searchValue: string, pageNumber: number, pageSize: number): ApiRequest =>
  async (dispatch, getState, { searchGames }) =>
    searchGames(searchValue, pageNumber, pageSize, getState().auth)
      .then(games => dispatch(addGames(games)));

export const { addGames } = gamesSlice.actions

export default gamesSlice.reducer