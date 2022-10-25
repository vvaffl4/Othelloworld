import { Accordion, AccordionDetails, AccordionSummary, Box, Paper, Typography } from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { FC } from 'react';
import { GameResult } from '../model/PlayerInGame';

interface ProfileGameHistoryItemProps {
	expanded: boolean;
	result: GameResult;
	playerOne: string;
	playerTwo: string;
	whiteCount: number;
	blackCount: number;
	onClick: () => void;
}

const ProfileGameHistoryItem: FC<ProfileGameHistoryItemProps> = ({ expanded, result, playerOne, playerTwo, whiteCount, blackCount, onClick }) => {

	var gameResultText = '';
	var gameResultColor = '';

	switch (result) {
		case GameResult.won:
			gameResultText = 'WIN';
			gameResultColor = 'success';
			break;
		case GameResult.lost:
			gameResultText = 'LOSE';
			gameResultColor = 'error';
			break;
		case GameResult.draw:
			gameResultText = 'DRAW';
			gameResultColor = 'neutral';
			break;
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
					>
						<Typography
							component="div"
							sx={{
								mt: '-10px',
								width: '80px',
								height: '80px',
								backgroundColor: (theme) => theme.palette[gameResultColor][theme.palette.mode],
								borderRadius: '50%',
								color: 'text.secondary',
								lineHeight: '80px',
								textAlign: 'center',
								fontSize: 24,
								fontWeight: 900
							}}
						>
							{gameResultText}
						</Typography>
					</Box>
					<Typography
						sx={{
							flexShrink: 0,
							width: '33%',
							lineHeight: '60px',
							textAlign: 'center'
						}}>
						{playerOne} vs {playerTwo}
					</Typography>
					<Typography
						sx={{
							width: '33%',
							flexShrink: 0,
							color: 'text.secondary',
							lineHeight: '60px',
						}}>
						White: {whiteCount} - Black: {blackCount}
					</Typography>
				</AccordionSummary>
				<AccordionDetails>
					<Typography>
						This is some important details about the game that was played years ago...
					</Typography>
				</AccordionDetails>
			</Accordion>
		</Paper>
	);
}

export default ProfileGameHistoryItem;