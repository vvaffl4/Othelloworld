import { Box, Container, Paper, Typography } from '@mui/material';
import { FC } from 'react';
import { useAppSelector } from '../store/Hooks';
import BadgeAvatar from './BadgeAvatar';

const Profile: FC = () => {
	const player = useAppSelector(state => state.auth.username);

	return (
		<Box
			component="div"
			sx={{
				position: 'absolute',
				top: '500px',
				left: 0,
				right: 0
			}}
		>
			<Container>
				<Paper>
					<BadgeAvatar countryIso="nl" />
					<Typography
						variant="h2"
					>
						{player}
					</Typography>
					<Typography>
						{ }
					</Typography>
				</Paper>
			</Container>
		</Box>
	);
}

export default Profile;