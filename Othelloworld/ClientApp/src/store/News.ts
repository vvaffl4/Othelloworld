import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface NewsItem {
	title: string;
	image: string;
	content: string;
}

const initialState: NewsItem[] = [
	{
		title: "News 1",
		image: "News 2",
		content: "News 3"
	},
	{
		title: "News 1",
		image: "News 2",
		content: "News 3"
	},
	{
		title: "News 1",
		image: "News 2",
		content: "News 3"
	}
];


export const newsSlice = createSlice({
	name: 'game',
	initialState,
	reducers: {
		addNewsItems: (state, action: PayloadAction<NewsItem[]>) => {
			state = [
				...state,
				...action.payload
			];
		},
	}
});

export const { addNewsItems } = newsSlice.actions

export default newsSlice.reducer