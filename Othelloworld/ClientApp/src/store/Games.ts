import { Table } from "@mui/material"
import { createSlice, PayloadAction } from "@reduxjs/toolkit"
import { ApiRequest } from "."
import Game from "../model/Game"


export type Library<T extends number | string, U> = {
  [i in T]: U
}

export type Table<T extends number | string, U> = {
  byId: Library<T, U>,
  allIds: T[]
}

interface GamesState extends Table<string, Game> { }

const initialState: GamesState = {
  byId: {},
  allIds: [ ]
}

const gamesSlice = createSlice({
  name: 'games',
  initialState,
  reducers: {
    addGames: (state, action: PayloadAction<Game[]>) => {
      state.byId = {
        ...state.byId,
        ...action.payload.reduce((byId, game) => ({ ...byId, [game.token]: game }), {})
      }
      state.allIds = [...new Set([
        ...state.allIds,
        ...action.payload.map(game => game.token)
      ])]
    }
  },
})


export const fetchGames = (): ApiRequest =>
  async (dispatch, getState, { getGames }) =>
    getGames(getState().auth)
      .then(games => dispatch(addGames(games)));

export const { addGames } = gamesSlice.actions

export default gamesSlice.reducer