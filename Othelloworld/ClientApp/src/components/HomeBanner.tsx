import { Box, Link, Typography } from '@mui/material';
import { FC } from 'react';
import { useAppSelector } from '../store/Hooks';

const HomeBanner: FC = () => {
	const authenticated = useAppSelector(state => state.auth.authenticated);

	return (
		<Box
			component="div"
			sx={{
				p: 4,
				backgroundColor: '#000000'
			}}
		>
			{authenticated
				? (
					<Typography variant="h3">
						<Link href="/play" underline="hover">Play now</Link> with people from all over the world!
					</Typography >
				)
				: (
					<Typography variant = "h3">
						<Link href="/register" underline="hover">Sign up</Link> for free, and play Othello (Reversi) with people all over the world!
					</Typography >
				)
			}
		</Box>
	);
}

export default HomeBanner;