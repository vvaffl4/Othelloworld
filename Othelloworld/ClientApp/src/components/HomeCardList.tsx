import { Box, Button, Grid, Link, Typography } from '@mui/material';
import { FC } from 'react';
import { useAppSelector } from '../store/Hooks';
import HomeCard from './HomeCard';

const HomeCardList: FC = () => {
	const news = useAppSelector(state => [...state.news]);

	return (
		<Grid
			container
			spacing={2}
			sx={{
				p: 2
			}}
		>
			{news.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
				.slice(0, 3)
				.map((newsItem, index) => (
					<Grid
						key={index}
						item
						xs={4}
					>
						<HomeCard {...newsItem} />
					</Grid>
				))}
			<Grid
				item
				xs={12}
				sx={{ alignContent: 'center' }}
			>
				<Button
					variant="outlined"
					sx={{ ml: 'auto' }}
				>
					Load more
				</Button>
			</Grid>
		</Grid>
	);
}

export default HomeCardList;