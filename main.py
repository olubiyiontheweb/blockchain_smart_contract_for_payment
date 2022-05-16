from fastapi import FastAPI

# initialize FastAPI

idBCServer = FastAPI(title="BlockChain Identity Server",  # root_path=settings.API_V1_STR,
    description="This blockchain identity server is used to register identify and authenticate users that meet the requirement of the smart contract.",
    version="1.0.0")

@idBCServer.get("/")
async def root():
    return {"message": "Hello World"}