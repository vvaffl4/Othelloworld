import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { ApiRequest } from ".";

interface Coords {
	lat: number,
	lng: number,
	altitude: number
}

interface Country {
	geometry: {
		coordinates: number[][][],
		mid: number[],
		type: string
	},
	properties: {
		NAME: string;
		ISO_A2: string;
	}
}

interface Polygon extends Coords, Country { };

interface WorldState {
	settings: {
		interactable: boolean;
		countrySelect: boolean;
		orbitControl: boolean;
		orbitAutoRotate: boolean;
		countrySelectMaxCount: number;
		show: boolean;
	},
	countries: Country[],
	selected: Country[],
	focus?: Coords
}

const initialState: WorldState = {
	settings: {
		interactable: false,
		countrySelect: false,
		orbitControl: false,
		orbitAutoRotate: true,
		countrySelectMaxCount: 1,
		show: true
	},
	countries: [],
	selected: [],
	focus: undefined
}

const worldSlice = createSlice({
	name: 'world',
	initialState,
	reducers: {
		changeWorldSettings: (state, action: PayloadAction<Partial<WorldState["settings"]>>) => {
			state.settings = {
				...state.settings,
				...action.payload
			}

			state.selected = state.selected.slice(0, state.settings.countrySelectMaxCount);
		},
		setCountries: (state, action: PayloadAction<Country[]>) => {
			state.countries = action.payload;
		},
		selectCountry: (state, action: PayloadAction<{ isoCode: string }>) => {
			const country = state.countries.find(country =>
				country.properties.ISO_A2.toLowerCase() === action.payload.isoCode.toLowerCase());

			if (country) {
				state.selected = [
					country,
					...state.selected
				].slice(0, state.settings.countrySelectMaxCount);
			}
		},
		deselectCountries: (state) => {
			state.selected = [];
		},
		focusCountry: (state, action: PayloadAction<{ isoCode: string, altitude: number }>) => {
			const mid = state.countries.find(country =>
				country.properties.ISO_A2.toLowerCase() === action.payload.isoCode.toLowerCase())
				?.geometry.mid;

			if (mid) {
				state.focus = {
					lng: mid[0],
					lat: mid[1],
					altitude: action.payload.altitude
				};
			}
		},
		blurCountry: (state) => {
			state.focus = undefined
		},
		selectAndFocusCountry: (state, action: PayloadAction<{ isoCode: string, altitude: number }>) => {
			const country = state.countries.find(country =>
				country.properties.ISO_A2.toLowerCase() === action.payload.isoCode.toLowerCase());

			if (country) {
				state.selected = [
					country,
					...state.selected
				].slice(0, state.settings.countrySelectMaxCount);
				state.focus = {
					lng: country.geometry.mid[0],
					lat: country.geometry.mid[1],
					altitude: action.payload.altitude
				};
			}
		}
	}
});

export const fetchCountries = (): ApiRequest => async (dispatch, getState, { fetchCountries }) =>
	getState().world.countries.length < 1
	&& fetchCountries()
		.then(countries =>
			dispatch(worldSlice.actions.setCountries(countries)));

export const {
	changeWorldSettings,
	selectCountry,
	deselectCountries,
	focusCountry,
	blurCountry,
	selectAndFocusCountry
} = worldSlice.actions
export type { Country, Coords, Polygon }

export default worldSlice.reducer