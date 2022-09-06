import { PaletteMode } from "@mui/material";
import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface UserInterfaceState {
	mode: PaletteMode
}

const initialState: UserInterfaceState = {
	mode: 'light'
}

const userInterfaceSlice = createSlice({
	name: 'ui',
	initialState,
	reducers: {
		setPaletteMode: (state, action: PayloadAction<PaletteMode>) => {
			state.mode = action.payload;
		}
	}
})

export const { setPaletteMode } = userInterfaceSlice.actions;

export default userInterfaceSlice.reducer;