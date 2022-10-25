import { Container, Grid, Paper, Typography } from '@mui/material';
import { FC } from 'react';
import HomeBanner from './HomeBanner';
import HomeCardList from './HomeCardList';

const Home: FC = () => {

	return (
		<Container>
			<Typography
				variant="h1"
				color="white"
				sx={{
					pt: 16,
					//padding: '100px 200px 0 0',
					fontWeight: 700,
					textAlign: 'right'
				}}
			>
				OTHELLO
			</Typography>
			<Typography
				variant="h2"
				color={ '#f06292' }
				sx={{
					textAlign: 'right'
				}}
			>
				WORLD
			</Typography>
			<Paper
				sx={{
					mt: 16,
					overflow: 'hidden'
				}}
			>
				<HomeBanner/>
				<HomeCardList/>
			</Paper>
		</Container>
	)
}

export default Home;