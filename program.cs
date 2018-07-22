/* ///////////////////////////////////
 / 
 /  Pravda smart program 0xWarriors
 /
 /  github: Alexander Belov (baadev)
 /  Date: 2018-07-21
 /  Copyright Alexander Belov
 /
*/ ///////////////////////////////////

using System;
using Com.Expload;

[Program]
class Warriors {


    /* ////////////////////
    // Definitions
    */ ////////////////////
    public Mapping<Bytes, bool> RootList = new Mapping<Bytes, bool>();                  

    public Mapping<Bytes, String> PlayerWarriors = new Mapping<Bytes, String>();        
    public Mapping<Bytes, int> PlayerBalance = new Mapping<Bytes, int>();               
    public Mapping<Bytes, int> PlayerWarriorCount = new Mapping<Bytes, int>();          

    public Mapping<String, Bytes> WarriorToOwner = new Mapping<String, Bytes>();        
    public Mapping<String, Bytes> WarriorAttributes = new Mapping<String, Bytes>();     
    public Mapping<String, int> WarriorType = new Mapping<String, int>();               
    public Mapping<String, int> WarriorLvl = new Mapping<String, int>();                
    public Mapping<String, int> WarriorId = new Mapping<String, int>();
    public Mapping<String, int> WarriorRare = new Mapping<String, int>();

    public Mapping<String, Bytes> AuctionWarriorOwner = new Mapping<String, Bytes>();   
    public Mapping<String, int> AuctionWarriorStartPrice = new Mapping<String, int>();
    public Mapping<String, int> AuctionWarriorEndPrice = new Mapping<String, int>();

    public Mapping<String, Bytes> AuctionPlayerBid = new Mapping<String,Bytes>();       
    public Mapping<String, int> AuctionPlayerBids = new Mapping<String, int>();

    public Mapping<int, Bytes> AuctionBidders = new Mapping<int, Bytes>();              
    public Mapping<String, int> AuctionBidBidders = new Mapping<String, int>();

    public Bytes sudo;
    public bool check = true;

    public int rootNum = 0;
    public int typesCount = 3;

    public int playerIds = 1;
    public int warriorIds = 1;

    public int CHEST_PRICE = 0;//!! 5

    public int commonChests = 600;
    public int rareChests = 300;
    public int epicChests = 90;
    public int legendaryChests = 10;

    public int auctionCountBidders = 0;



    /* ////////////////////
    // pseudoConstructor of class
    */ ////////////////////
    public bool initWarriors() 
    {
        if(check){
            Bytes sender = Info.Sender();
            sudo = sender;
            RootList.put(sender, true);
            rootNum += 1;

            return RootList.get(sender);
        }
        check = false;
        return RootList.get(sudo);
    }


    /* ////////////////////
    // Root metods
    */ ////////////////////
    public String addRoot(Bytes newRoot)
    {
        Bytes sender = Info.Sender();
        if (RootList.exists(sender) && RootList.getDefault(sender, false))
        {
            RootList.put(newRoot, true);
            rootNum += 1;

            return "Success";
        }
        return "Something went wrong, check your permissions.";
    }

    public String removeRoot(Bytes removingRoot)
    {
        Bytes sender = Info.Sender();
        if (RootList.exists(sender) && RootList.getDefault(sender, false) && RootList.exists(removingRoot) && 1 < rootNum && (!(removingRoot == sudo)))
        {
            RootList.put(removingRoot, false);
            rootNum -= 1;

            return "Success";
        }
        return "Something went wrong, check your permissions.";
    }

    public String giveCoins(Bytes player, int coins)
    {
        Bytes sender = Info.Sender();
        if (RootList.exists(sender) && RootList.getDefault(sender, false))
        {
            int currentBalance = PlayerBalance.getDefault(player, 0);
            PlayerBalance.put(player, currentBalance += coins);

            return "Success";
        }
        return "Something went wrong, check your permissions.";
    }

    /* ////////////////////
    // Chests
    */ ////////////////////
    public void changeChestPrice(int newPrice)
    {
        Bytes sender = Info.Sender();        
        if (RootList.exists(sender) && RootList.getDefault(sender, false))
            CHEST_PRICE = newPrice;
    }

    private int rarityOfChest() 
    {
        int rarityValue = ((System.Convert.ToInt32(Info.Sender()) * System.Convert.ToInt32(StdLib.Ripemd160(System.Convert.ToString(commonChests + rareChests + epicChests + legendaryChests + 1))))) % 100;
        if (rarityValue < 60 && 0 < commonChests)
        {
            commonChests -= 1;
            return 1;
        }
        else if (rarityValue < 90 && 0 < rareChests)
        {
            rareChests -= 1;
            return 2;
        }
        else if (rarityValue < 99 && 0 < epicChests)
        {
            epicChests -= 1;
            return 3;
        }
        else if (0 < legendaryChests)
        {
            legendaryChests -= 1;
            return 4;
        }
        return 0;
    }

    public String newChestWarrior() 
    {
        Bytes sender = Info.Sender();
        int balanceSender = _balanceOf(sender);
        int warriorCountSender = _warriorCount(sender);
        int rarity = rarityOfChest();
        String warriorsList = PlayerWarriors.getDefault(sender,"");

        if(rarity > 0)
        {
            if (((CHEST_PRICE - 1) < balanceSender) && (warriorCountSender < 5))
            {
                 int typeOfWarrior = (System.Convert.ToInt32(StdLib.Ripemd160(System.Convert.ToString(warriorIds))) % typesCount);
                    if (typeOfWarrior < 0) 
                        typeOfWarrior = 0; 
                    else 
                    if (typeOfWarrior > typesCount - 1)
                        typeOfWarrior = typesCount - 1;

                 String warriorName = System.Convert.ToString(warriorIds) + (System.Convert.ToString(typeOfWarrior) + ".");

                if (typeOfWarrior == 0)
                {
                    int str = 3 * rarity;
                    int endur = 5 * rarity;
                    int agil = 3 * rarity;
                    int luck = 2 * rarity;
                    privateSetupWarriorAttributes(warriorName, str, endur, agil, luck);
                    WarriorType.put(warriorName, 0);
                    WarriorLvl.put(warriorName, 0);
                    WarriorId.put(warriorName, warriorIds);                
                } else 
                if (typeOfWarrior == 1)
                {
                    int str = 1 * rarity;
                    int endur = 2 * rarity;
                    int agil = 4 * rarity;
                    int luck = 3 * rarity;
                    privateSetupWarriorAttributes(warriorName, str, endur, agil, luck);
                    WarriorType.put(warriorName, 1);
                    WarriorLvl.put(warriorName, 0);
                    WarriorId.put(warriorName, warriorIds);                    
                } else 
                if (typeOfWarrior == 2)
                {
                    int str = 5 * rarity;
                    int endur = 3 * rarity;
                    int agil = 1 * rarity;
                    int luck = 4 * rarity;
                    privateSetupWarriorAttributes(warriorName, str, endur, agil, luck);
                    WarriorType.put(warriorName, 2);
                    WarriorLvl.put(warriorName, 0);
                    WarriorId.put(warriorName, warriorIds);
                }

                if (warriorCountSender == 0)
                    warriorsList = warriorName;
                else
                    warriorsList = warriorsList + warriorName;

                warriorIds += 1;            
                warriorCountSender += 1;
                int newPlayerBalance = balanceSender - CHEST_PRICE;

                WarriorRare.put(warriorName, rarity);
                PlayerWarriorCount.put(sender, warriorCountSender);
                PlayerBalance.put(sender, newPlayerBalance);
                WarriorToOwner.put(warriorName, sender);
                PlayerWarriors.put(sender, warriorsList);

                return "Success givving new warrior from chest.";
            }
            return "Too much warriors.";
        } else
            return "Out of stock.";
    }


    /* ////////////////////
    // Setup attributes
    */ ////////////////////
    public String setupWarriorAttributes(String warrior, int strength, int endurance, int agility, int luck)
    {
        if (WarriorToOwner.exists(warrior) && RootList.exists(Info.Sender()) && RootList.getDefault(Info.Sender(), false))
        {
            byte[] attributes = new byte[4];

            attributes[0] = Convert.ToByte(strength); 
            attributes[1] = Convert.ToByte(endurance); 
            attributes[2] = Convert.ToByte(agility); 
            attributes[3] = Convert.ToByte(luck); 

            WarriorAttributes.put(warrior, new Bytes(attributes));

            return "Setup completed with success.";
        }
        return "Something went wrong. Check root permissions and warrior name.";
    }

    private void privateSetupWarriorAttributes(String warrior, int strength, int endurance, int agility, int luck)
    {
        byte[] attributes = new byte[4];

        attributes[0] = Convert.ToByte(strength); 
        attributes[1] = Convert.ToByte(endurance); 
        attributes[2] = Convert.ToByte(agility); 
        attributes[3] = Convert.ToByte(luck); 

        WarriorAttributes.put(warrior, new Bytes(attributes));
    }


    /* ////////////////////
    // Transfer
    */ ////////////////////
    public String transferWarrior(Bytes receiver, String warrior)
    {
        int warCount = PlayerWarriorCount.getDefault(receiver, 0);
        Bytes sender = Info.Sender();
        if (WarriorToOwner.get(warrior) == sender)
        {
            if (warCount < 5)
            {
                warCount++;
                int newSenderCount = PlayerWarriorCount.getDefault(sender, 0) - 1;

                PlayerWarriorCount.put(sender, newSenderCount);
                WarriorToOwner.put(warrior, receiver);
                PlayerWarriorCount.put(receiver, warCount);

                return "Success transaction.";
            }
            return "Recipient has the maximum number of warriors.";
        }
        return "Something went wrong. Maybe you are not owner of this warrior?.";
    }


    /* ////////////////////
    // Auction
    */ ////////////////////
    public String auctionStart(String warrior, int startPrice, int endPrice)
    {
        Bytes sender = Info.Sender();
        if (startPrice > 0 && endPrice > startPrice)
        {
            if (WarriorToOwner.get(warrior) == sender)
            {
                AuctionWarriorOwner.put(warrior, sender);

                AuctionWarriorStartPrice.put(warrior, startPrice);
                AuctionWarriorEndPrice.put(warrior, endPrice);

                WarriorToOwner.put(warrior, new Bytes(0));
                int countWarrior = PlayerWarriorCount.get(sender) - 1;
                PlayerWarriorCount.put(sender, countWarrior);

                return "Success added on auction.";
            }
            return "You are not owner of this warrior.";
        }
        return "Bad data.";
    }

    public String auctionCancel(String warrior)
    {
        Bytes sender = Info.Sender();
        if (AuctionWarriorOwner.get(warrior) == sender)
        {
            AuctionWarriorStartPrice.put(warrior, 0);
            AuctionWarriorEndPrice.put(warrior, 0);
            AuctionWarriorOwner.put(warrior, new Bytes(0));

            WarriorToOwner.put(warrior, sender);

            return "Success removed from auction.";
        }
        return "You are not owner of this warrior.";
    }

    public String auctionBid(String warrior, int bid)
    {
        Bytes sender = Info.Sender();
        if (AuctionWarriorOwner.exists(warrior))
        {
            int price = AuctionWarriorStartPrice.get(warrior);
            int _playerBalance = PlayerBalance.getDefault(sender, 0);
            if (_playerBalance > bid + price - 1)
            {
                int endPrice = AuctionWarriorEndPrice.get(warrior);
                int newPrice = price + bid;
                if (newPrice > endPrice - 1)
                {

                    AuctionWarriorOwner.put(warrior, new Bytes(0));
                    WarriorToOwner.put(warrior, sender);

                    PlayerBalance.put(sender, _playerBalance - newPrice);

                    return "You successfully bought this warrior.";
                } 
                else
                {
                    PlayerBalance.put(sender, _playerBalance - bid);
                    AuctionPlayerBid.put(warrior, sender);

                    String mapkey = warrior + sender;                           
                    int currentBid = AuctionPlayerBids.getDefault(mapkey, 0);   
                    AuctionPlayerBids.put(mapkey, currentBid + bid);            

                    AuctionWarriorStartPrice.put(warrior, newPrice);

                    return "Success bid.";
                }
            }
            return "Not enough coins.";
        }
        return "Something went wrong. Check warrior name.";
    }


    /* ////////////////////
    // Getters
    */ ////////////////////
    public String getWarriorBio(String warrior)
    {   
        if (WarriorToOwner.exists(warrior))
        {
            Bytes attributes = WarriorAttributes.get(warrior);
            Bytes owner = WarriorToOwner.get(warrior);

            String id = System.Convert.ToString(WarriorId.get(warrior));
            String type = System.Convert.ToString(WarriorType.get(warrior));
            String rarity = System.Convert.ToString(WarriorRare.get(warrior));
            String lvl = System.Convert.ToString(WarriorLvl.get(warrior));

            String at1 = System.Convert.ToString(attributes[0]);
            String at2 = System.Convert.ToString(attributes[1]);
            String at3 = System.Convert.ToString(attributes[2]);
            String at4 = System.Convert.ToString(attributes[3]);
            String strOwner = "";

            for(int i = 0;i < 32; i++)
            {
                String n = System.Convert.ToString(owner[i]);
                strOwner += n;
            }

            String gg = "{ \"id\": " + id;
            gg += ", \"type\":" + type;
            gg += ", \"rarity\":" + rarity;
            gg += ", \"birthDate\":" + "0";
            gg += ", \"level\":" + lvl;
            gg += ", \"owner\":\"" + strOwner;
            gg += "\"" + ", \"attributes\": ["; 
            gg += at1 + ","; 
            gg += at2 + ","; 
            gg += at3 + ","; 
            gg += at4 + "]}";

            return gg;
        }
        return "Something went wrong. Check warrior name";
    }


    public int idOf(String warrior) {
        return WarriorId.getDefault(warrior, 0);
    }

    public int balanceOf(Bytes tokenOwner) 
    {
        Bytes sender = Info.Sender();
        if(RootList.exists(sender) && RootList.getDefault(sender, false) || sender == sudo || sender == Info.Sender())
            return PlayerBalance.getDefault(tokenOwner, 0);         
        else
            return 0;
    }

    public int warriorCount(Bytes player) 
    {
        Bytes sender = Info.Sender();
        if(RootList.exists(sender) && RootList.getDefault(sender, false) || sender == sudo || sender == Info.Sender())
            return PlayerWarriorCount.getDefault(player, 0);        
        else
            return 0;
    }

    public String getterWarriorList(Bytes player){
        return PlayerWarriors.getDefault(player, "You haven't any warriors");
    }
    public void setterWarriorList(Bytes player, String newList)
    {
        Bytes sender = Info.Sender();
        if(RootList.exists(sender) && RootList.getDefault(sender, false) || sender == sudo)
            PlayerWarriors.put(player, newList);
    }
    
    private int _warriorCount(Bytes player) {
        return PlayerWarriorCount.getDefault(player, 0);       
    }

    private int _balanceOf(Bytes tokenOwner) {
        return PlayerBalance.getDefault(tokenOwner, 0);         
    }

}

class MainClass {
    public static void Main() {}
}
