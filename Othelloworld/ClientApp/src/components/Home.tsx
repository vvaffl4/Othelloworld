import { Container, Grid, Paper, Typography } from '@mui/material';
import { FC } from 'react';
import HomeCard from './HomeCard';

const Home: FC = () => {
	return (
		<Container>
			<Typography
				variant="h1"
				color="white"
				sx={{
					padding: '100px 200px 0 0',
					fontWeight: 700,
					textAlign: 'right'
				}}
			>
				OTHELLO
			</Typography>
			<Paper
				sx={{
					m: 15,
					overflow: 'hidden'
				}}
			>
				<Grid container spacing={2}>
					<Grid item xs={4}>
						<HomeCard/>
					</Grid>
					<Grid item xs={4}>
						<HomeCard />
					</Grid>
					<Grid item xs={4}>
						<HomeCard />
					</Grid>
					<Grid item xs={12}>
						Click here for more
					</Grid>
				</Grid>
			</Paper>
		</Container>
	)
}

export default Home;