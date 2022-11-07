import { Accordion, AccordionDetails, AccordionSummary, Box, Paper, Typography } from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { FC } from 'react';
import { useAppDispatch, useAppSelector } from '../store/Hooks';
import { joinGame } from '../store/Game';
import Game from '../model/Game';

interface GameListItemProps {
	game: Game;
	expanded: boolean;
	onClick: () => void;
}

const GameListItem: FC<GameListItemProps> = ({ game, expanded, onClick }) => {
	const dispatch = useAppDispatch()

	const host = game.players.find(player => player && player.isHost)!;

	const handlePlayClick = () => {
		dispatch(joinGame(game.token));
	}

	return (
		<Paper
			variant="outlined"
			sx={{
				backgroundColor: '#121212'
			}}
			onClick={onClick}
		>
			<Accordion
				expanded={expanded}
				sx={{
					background: 'transparent',
					overflow: 'hidden'
				}}
			>
				<AccordionSummary
					expandIcon={<ExpandMoreIcon />}
					aria-controls="panel1bh-content"
					id="panel1bh-header"
					sx={{
						minHeight: '60px !important',
						'.MuiAccordionSummary-content': {
							m: '0 !important',
							height: '60px'
						}
					}}
				>
					<Box
						component="div"
						sx={{
							flexShrink: 0,
							mx: '20px',
							height: '100%'
						}}
						onClick={handlePlayClick}
					>
						<Typography
							component="div"
							sx={{
								mt: '-10px',
								width: '80px',
								height: '80px',
								backgroundColor: (theme) => theme.palette.primary[theme.palette.mode],
								borderRadius: '50%',
								color: 'text.secondary',
								lineHeight: '80px',
								textAlign: 'center',
								fontSize: 24,
								fontWeight: 900
							}}
						>
							Play
						</Typography>
					</Box>
					<Typography
						sx={{
							width: '33%',
							flexShrink: 0,
							color: 'text.primary',
							lineHeight: '60px',
						}}>
						Game - {game.name}
					</Typography>
					<Typography
						sx={{
							flexShrink: 0,
							width: '33%',
							color: 'text.secondary',
							lineHeight: '60px',
							textAlign: 'center'
						}}>
						Player - {host.player.username}
					</Typography>
				</AccordionSummary>
				<AccordionDetails>
					<Typography>
						{game.description}
					</Typography>
				</AccordionDetails>
			</Accordion>
		</Paper>
	);
}

export default GameListItem;