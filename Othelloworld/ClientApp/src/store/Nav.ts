import { Table } from "@mui/material"
import { createSlice, PayloadAction } from "@reduxjs/toolkit"
import { ApiRequest } from "."
import Game from "../model/Game"


interface NavState {
  current: string;
  all: string[];
}

const initialState: NavState = {
  current: 'home',
  all: [ 'home', 'play' ]
}

const navSlice = createSlice({
  name: 'nav',
  // `createSlice` will infer the state type from the `initialState` argument
  initialState,
  reducers: {
    setPage(state, action: PayloadAction<{ page: string }>) {
      state.current = action.payload.page;
    }
  },
})

export const { setPage } = navSlice.actions

export default navSlice.reducer